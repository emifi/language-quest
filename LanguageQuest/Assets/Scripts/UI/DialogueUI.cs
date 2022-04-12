using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Subtegral.DialogueSystem.DataContainers;

public class DialogueUI : MonoBehaviour
{
    public Canvas dialogueUI;
    public static NotebookObject notebook; //Used to add keywords to dictionary
    private TMP_Text dialogue;
    private static GameObject decisionSpace; //The physical UI space
    private static TMP_Text decisionA;
    private static TMP_Text decisionB;
    private static TMP_Text decisionC;
    private static TMP_Text decisionD;
    private static DialogueContainer dialogueContainer; //Dialogue tree
    private static NpcNavMesh currNPC; 
    private static int textPos; //Controls text reveal
    private float textDelay; //Controls text reveal speed
    private static string displayedText; //Revealed text
    public static bool lineComplete = false;
    public static bool speechComplete = true;

    private static NodeLinkData narrativeData; //Current dialogue node
    private static List<NodeLinkData> choices; //Is set to hold dialogue choices
    private static string fullText; //Full dialogue line

    private static TMP_Text[,] decisionGrid; // Array representation of dialogue space
    private static int numChoices; //Used to hold count of choices
    private static int currChoiceX; //Used for highlight/selection of choices
    private static int currChoiceY;

    private static bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        notebook = Resources.Load<NotebookObject>("Inventory/Player Notebook");
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueUI.enabled = false;
        isEnabled = false;
        textPos = 0;
        displayedText = "";
        narrativeData = null;
        choices = null;
        fullText = null;
        dialogue = dialogueUI.transform.Find("DialogueBox/Paper/Text").GetComponent<TMP_Text>();
        decisionSpace = GameObject.Find("SelectionSpace");
        textDelay = .15f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled){
            dialogueUI.enabled = true;
        }

        if(narrativeData==null||fullText==null){
            lineComplete = false;
            speechComplete = true;
            narrativeData = null;
            choices = null;
            fullText = null;
            dialogueUI.enabled = false;
            isEnabled = false;
            displayedText = "";
            textPos = 0;
            return;
        }
        if(Time.time-textDelay>.05f&&!speechComplete&&textPos<fullText.Length){
            textDelay = Time.time;
            if(fullText[textPos]=='<'){
                int endPos = fullText.IndexOf(">",textPos);
                displayedText+=fullText.Substring(textPos,endPos-textPos);
                textPos+=(endPos-textPos);
            }
            if(textPos<fullText.Length){ //May look redundant, but ensures that upon <tag> element disappearing, a new character
                displayedText+=fullText[textPos]; //is added in its absence
                textPos++;
            }
        }
        if(textPos>=fullText.Length){
            lineComplete = true;
        }

        if(currNPC!=null&&currNPC.getType()!=NpcNavMesh.NpcType.Proximity&&currNPC.getType()!=NpcNavMesh.NpcType.Stationary){
            currNPC.stopMovement();
        }
        
        dialogue.text = displayedText;
    }

    public static void turnPage(){
        if(choices.Count()==0){
            nextDialogue(null);
            return;
        }
        if(choices.Count()==1){
            nextDialogue(choices[0].TargetNodeGUID);
            return;
        }
        decisionGrid[currChoiceY,currChoiceX].color = new Color(.078f,.078f,.078f);
        nextDialogue(choices[(currChoiceY*decisionGrid.GetLength(1))+currChoiceX].TargetNodeGUID);
    }

    public static void skip(){
        displayedText = fullText;
        textPos = displayedText.Count();
        lineComplete = true;
    }

    private static void nextDialogue(string narrativeDataGUID){
        if(narrativeDataGUID==null){
            narrativeData = null;
            return;
        }
        lineComplete = false;
        displayedText = "";
        textPos=0;
        fullText = dialogueContainer.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        

        //ADD ANY REPLACEMENT VALUES YOU WOULD LIKE HERE.
        //MAKE THEM THE SAME AS THEY ARE IN DIALOGUE GRAPHS.
        fullText = fullText.Replace("{replacename}",("<#3afa14>" + currNPC.name + "<#141414>"));

        //Objective Parsing
        fullText = objectiveParse(fullText,currNPC);

        //Term Parsing (no print variation)
        fullText = addTermParse(fullText);

        //Term Parsing (print variation)
        fullText = addTermPrintParse(fullText);


        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID).ToList();
        numChoices = choices.Count();

        //Next Dialogue Parsing
        fullText = goToParse(fullText,currNPC);


        if(numChoices==0){
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else if(numChoices==1){
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else{
            decisionSpace.SetActive(true);
            int copyNumChoices = numChoices;
            Debug.Log(numChoices);

            decisionGrid = new TMP_Text[2,2]; //Feel free to resize decision grid based on your maximum # of responses. 
                                      //Methods will react to this without issue
            decisionGrid[0,0] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row1/A").GetComponent<TMP_Text>();
            decisionGrid[0,1] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row1/B").GetComponent<TMP_Text>();
            decisionGrid[1,0] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row2/C").GetComponent<TMP_Text>();
            decisionGrid[1,1] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row2/D").GetComponent<TMP_Text>();
            
            int pos = 0;
            foreach(TMP_Text t in decisionGrid){
                    if(copyNumChoices<=0){
                        t.enabled = false;
                    }else{
                        t.enabled = true;
                        int posX = pos%decisionGrid.GetLength(1);
                        int posY = pos/decisionGrid.GetLength(1);
                        decisionGrid[posY,posX].text = choices[pos].PortName;
                    }
                    copyNumChoices--;
                    pos++;
            }

        }

        isEnabled = true;

        if(decisionGrid!=null){
            currChoiceX = 0;
            currChoiceY = 0;
            decisionGrid[0,0].color = Color.cyan;
        }
    }

    public static void scrollX(int i){//Scroll through choices with A and D. Wraparound on overflow.
        if(numChoices<=1){
            return;
        }
        decisionGrid[currChoiceY,currChoiceX].color = new Color(.078f,.078f,.078f);;
        currChoiceX += i;
        if(currChoiceX<0){
            currChoiceX = decisionGrid.GetLength(1)-1;
            currChoiceY -= 1;
            if(currChoiceY<0){
                currChoiceY = decisionGrid.GetLength(0)-1;
            }
        }else if(currChoiceX>=decisionGrid.GetLength(1)||decisionGrid[currChoiceY,currChoiceX].enabled==false){
            currChoiceX = 0;
            currChoiceY += 1;
            if(currChoiceY>=decisionGrid.GetLength(0)){
                currChoiceY = 0;
            }
        }
        while(decisionGrid[currChoiceY,currChoiceX].enabled==false){
            currChoiceX-=1;
            if(currChoiceX<0){
                currChoiceX = decisionGrid.GetLength(1)-1;
                currChoiceY -= 1;
            }
        }
        decisionGrid[currChoiceY,currChoiceX].color = Color.cyan;
    }

    public static void scrollY(int i){ //Scroll through choices with W and S. Wraparound on overflow.
        if(numChoices<=1){
            return;
        }
        decisionGrid[currChoiceY,currChoiceX].color = new Color(.078f,.078f,.078f);;
        currChoiceY += i;
        if(currChoiceY<0){
            currChoiceY = decisionGrid.GetLength(0)-1;
        }else if(currChoiceY>=decisionGrid.GetLength(0)||decisionGrid[currChoiceY,currChoiceX].enabled==false){
            currChoiceY = 0;
        }
        while(decisionGrid[currChoiceY,currChoiceX].enabled==false){
            currChoiceY-=1;
        }
        decisionGrid[currChoiceY,currChoiceX].color = Color.cyan;
    }

    public static void initScript(NpcNavMesh n){
        speechComplete = false;
        currNPC = n;
        dialogueContainer = currNPC.getCurrDialogue();
        narrativeData = dialogueContainer.NodeLinks.First(); //Entrypoint node
        nextDialogue(narrativeData.TargetNodeGUID);
    }

    public static bool textComplete(){
        return lineComplete;
    }

    public static bool dialogueComplete(){
        return speechComplete;
    }

    public static NpcNavMesh.NpcType getNpcType(){
        return currNPC.getType();
    }

    private static string objectiveParse(string fullText, NpcNavMesh currNPC){
        int startPos = fullText.IndexOf("ADDQUESTS{");
        int addLen = 10; //Length of ADDQUESTS{
        if(startPos==-1){
            startPos = fullText.IndexOf("ADDQUEST{");
            addLen = 9; //Length of ADDQUEST{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] questList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            List<Objective> newObjs = new List<Objective>();
            for(int i = 0; i<questList.Count()-1;i++){
                Debug.Log(questList[i]);
                newObjs.Add(Resources.Load<Objective>("Objective System/"+questList[i].Trim()));
            }
            ObjectiveDialogueGroup newObjGroup = new ObjectiveDialogueGroup(newObjs,currNPC,int.Parse(questList[questList.Count()-1]));
            GameObject.Find("Game Controller").GetComponent<GameController>().CreateGrouping(newObjGroup);
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText;
    }

    private static string addTermPrintParse(string fullText){
        int startPos = fullText.IndexOf("ADDTERMSPRINT{");
        int addLen = 14; //Length of ADDQUESTS{
        if(startPos==-1){
            startPos = fullText.IndexOf("ADDTERMPRINT{");
            addLen = 13; //Length of ADDQUEST{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] termList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            ItemObject item = null;
            ItemObject item2 = null;
            int count = termList.Count();
            string termstoString = "<#51fafc>";
            for(int i = 0; i<count;i++){
                item = Resources.Load<ItemObject>("Items/"+termList[i].Trim());
                if(count==1){
                    termstoString+=(item.englishName + "(" + item.nativeName + ")");
                }else if(i==0&&count==2){
                    item2 = Resources.Load<ItemObject>("Items/"+termList[i+1].Trim());
                    termstoString+=(item.englishName + "(" + item.nativeName + ") <#141414>and <#51fafc>" + item2.englishName + "(" + item2.nativeName + ")");
                    notebook.AddItem(item2);
                }else{
                    if(i==count-2){
                        item2 = Resources.Load<ItemObject>("Items/"+termList[i+1].Trim());
                        termstoString+=(item.englishName + "(" + item.nativeName + ")<#141414>, and <#51fafc>" + item2.englishName + "(" + item2.nativeName + ")");
                        notebook.AddItem(item2);
                    }else if(i<count-2){
                        termstoString+=(item.englishName + "(" + item.nativeName + ")<#141414>, <#51fafc>");
                    }
                }
                notebook.AddItem(item);
            }
            termstoString += "<#141414>";

            fullText = fullText.Substring(0,startPos) + termstoString  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText;
    }

    private static string addTermParse(string fullText){
        int startPos = fullText.IndexOf("ADDTERMS{");
        int addLen = 9; //Length of ADDQUESTS{
        if(startPos==-1){
            startPos = fullText.IndexOf("ADDTERM{");
            addLen = 8; //Length of ADDQUEST{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] termList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            int count = termList.Count();
            for(int i = 0; i<count;i++){
                notebook.AddItem(Resources.Load<ItemObject>("Items/"+termList[i].Trim()));
            }

            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText;
    }

    private static string goToParse(string fullText, NpcNavMesh currNPC){
        int startPos = fullText.IndexOf("GOTO{");
            int addLen = 5; //Length of GOTO{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue
                currNPC.setDialoguePointer(int.Parse(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))));

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText;
    }
}
