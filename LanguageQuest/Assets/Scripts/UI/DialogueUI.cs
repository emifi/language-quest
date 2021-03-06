using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    public Canvas dialogueUI;
    public static NotebookObject notebook; //Used to add keywords to dictionary
    public static InventoryObject inventory; //used to manipulate inventory
    private static TMP_Text dialogue;
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
    public string mainColor; //public facing
    public string npcColor;
    public string itemColor;
    public string choiceColor;
    private static string playerName;
    private static string mainCol; //private/static use
    private static string npcCol;
    private static string itemCol;
    private static string choiceCol;
    private static string mainColForDialogue;

    private static NodeLinkData narrativeData; //Current dialogue node
    private static List<NodeLinkData> choices; //Is set to hold dialogue choices
    private static string fullText; //Full dialogue line

    private static TMP_Text[,] decisionGrid; // Array representation of dialogue space
    private static int numChoices; //Used to hold count of choices
    private static int currChoiceX; //Used for highlight/selection of choices
    private static int currChoiceY;

    private static bool isEnabled;
    private static string sceneSwitch = null;
    private static int timeChange = -1; 
    private static float score = 0;
    private static bool lineChange = false;
    private static string npcTitle = "";

    private static ObjectiveSystem objectiveSystem;

    // Start is called before the first frame update
    void Start()
    {
        //DataStructs.populateGlobalDictionary();
        notebook = DataStructs.notebook;
        inventory = DataStructs.inventory;
        //playerName = dataStructs.playerName;

        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();

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

        if(mainColor.Length==7){
            mainCol = "<"+mainColor+">";
            mainColForDialogue = mainColor;
        }else{ //Defaults
            mainCol = "<#141414>";
            mainColForDialogue = "#141414";
        }
        
        if(npcColor.Length==7){
            npcCol = "<"+npcColor+">";
        }else{ //Defaults
            npcCol = "<#3afa14>";
        }

        if(itemColor.Length==7){
            itemCol = "<"+itemColor+">";
        }else{ //Defaults
            itemCol = "<#51fafc>";
        }

        if(choiceColor.Length==7){
            choiceCol = choiceColor;
        }else{ //Defaults
            choiceCol = "cyan";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled){
            dialogueUI.enabled = true;
        }

        if(narrativeData==null||fullText==null){ //If null, the end of map has been reached
            lineComplete = false;
            speechComplete = true;
            narrativeData = null;
            choices = null;
            fullText = null;
            dialogueUI.enabled = false;
            isEnabled = false;
            displayedText = "";
            textPos = 0;
            if(sceneSwitch!=null){
                StartCoroutine(Wait(3.0f,sceneSwitch));
                sceneSwitch=null;
            }

            if(timeChange>=0){
                StartCoroutine(ChangeTime(2.0f,timeChange));
                timeChange = -1;
                }   
            return;
            }
        if(Time.time-textDelay>.05f&&!speechComplete&&textPos<fullText.Length){ //Write text one char at a time based on textDelay
            textDelay = Time.time;
            if(fullText[textPos]=='<'){ //Exclude Html TAGs from char-based additions
                int endPos = fullText.IndexOf(">",textPos);
                displayedText+=fullText.Substring(textPos,endPos-textPos+1);
                textPos+=(endPos-textPos+1);
            }
            if(textPos<fullText.Length&&fullText[textPos]!='<'){ //May look redundant, but ensures that upon <tag> element disappearing, a new character
                displayedText+=fullText[textPos]; //is added in its absence
                textPos++;
            }
        }
        if(textPos>=fullText.Length){
            lineComplete = true;
        }

        if(currNPC!=null&&currNPC.getType()!=NpcNavMesh.NpcType.Proximity&&currNPC.getType()!=NpcNavMesh.NpcType.Stationary){
            currNPC.stopMovement(); //Ensure NPC does not run away while speaking (save for proximity/stationary types)
        }
        
        dialogue.text = displayedText;
    }

    IEnumerator Wait(float t, string scene) {
        GameObject.Find("Fade").GetComponent<Fade>().FadeOut(Color.black, 0.0f, t);
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(scene);
    }

    IEnumerator ChangeTime(float t, int dayTime) {
        GameObject.Find("Fade").GetComponent<Fade>().FadeOut(Color.black, 0.0f, t);
        yield return new WaitForSeconds(t);
        ChangeTimeOfDay.SetTimeWithInt(dayTime);
        yield return new WaitForSeconds(t);
        GameObject.Find("Fade").GetComponent<Fade>().FadeIn(Color.black, 0.0f, t);
    }

    public static void turnPage(){ //Proceed to next dialogue node
        lineChange = false;
        if(choices.Count()==0){ //End dialogue
            nextDialogue(null);
            return;
        }
        if(choices.Count()==1){ //Proceed to next dialogue
            nextDialogue(choices[0].TargetNodeGUID);
            return;
        }
        decisionGrid[currChoiceY,currChoiceX].color = new Color(.078f,.078f,.078f); //Reset decision grid colors
        nextDialogue(choices[(currChoiceY*decisionGrid.GetLength(1))+currChoiceX].TargetNodeGUID); //Proceed to chosen dialogue
    }

    public static void skip(){ //Skip text scrolling
        displayedText = mainCol+fullText;
        textPos = displayedText.Count();
        lineComplete = true;
    }

    private static void nextDialogue(string narrativeDataGUID){ //Display and set up new dialogue node
        if(narrativeDataGUID==null){
            narrativeData = null;
            return;
        }
        lineComplete = false;
        displayedText = mainCol+""; //Start text with chosen main color
        textPos=0;
        fullText = dialogueContainer.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        

        //ADD ANY REPLACEMENT VALUES YOU WOULD LIKE HERE.
        //MAKE THEM THE SAME AS THEY ARE IN DIALOGUE GRAPHS.
        if(currNPC != null){
            fullText = fullText.Replace("{npctitle}",("<size=150%><#060606>" +currNPC.name + mainCol + "</size>")); //For START of dialogue
            fullText = fullText.Replace("{npcname}",(npcCol + currNPC.name + mainCol)); //For any reference in dialogue
            if(fullText.Length>currNPC.name.Length+2&&(fullText.Trim().Substring(0,currNPC.name.Length+2))==($"{{"+$"{currNPC.name}"+$"}}")){
                Debug.Log(currNPC.name);
                fullText = fullText.Replace(($"{{"+$"{currNPC.name}"+$"}}"),("<size=150%><#060606>" +currNPC.name + mainCol + "</size>")); //For START of dialogue
            }
            if(fullText.Contains("<size=150%><#060606>" +currNPC.name + mainCol + "</size>")){
                npcTitle = "<size=150%><#060606>" +currNPC.name + mainCol + "</size>";
                displayedText += npcTitle;
                textPos+=npcTitle.Length;
            }
        }

        fullText = fullText.Replace("{playername}",DataStructs.playerName);

        fullText = fullText.Replace("{maincolor}",mainCol);
        fullText = fullText.Replace("{npccolor}",npcCol);
        fullText = fullText.Replace("{itemcolor}",itemCol);
        fullText = fullText.Replace("{choicecolor}",choiceCol);


        //Objective Parsing - ADDQUEST{quest0,quest1...questN,dialoguePTR}/ADDQUEST{quest0,quest1...questN,dialoguePTR}
        for (int i = 0; i < 5; i++) {
            fullText = objectiveParse(fullText,currNPC);
        }

        //Objective Parsing -   ADDQUEST++{quest0,quest1...questN,NPC0:dialoguePTR...NPC1:dialoguePTR}
        //                      ADDQUESTS++{quest0,quest1...questN,NPC0:dialoguePTR...NPC1:dialoguePTR}
        for (int i = 0; i < 5; i++) {
            fullText = objectiveParseMultNPC(fullText); //++{Collect-jak-3,MoveTo-NW,Trigger-Book,NPC (1):2, NPC (2):2}
        }

        //Hidden Objective Parsing - HIDDENQUESTS{quest0,quest1...questN,dialoguePTR}/HIDDENQUEST{quest0,quest1...questN,dialoguePTR}
        for (int i = 0; i < 5; i++) {
            fullText = hiddenObjectiveParse(fullText,currNPC);
        }

        //Hidden Objective Parsing -   HIDDENQUESTS++{quest0,quest1...questN,NPC0:dialoguePTR...NPC1:dialoguePTR}
        //                      HIDDENQUESTS++{quest0,quest1...questN,NPC0:dialoguePTR...NPC1:dialoguePTR}
        for (int i = 0; i < 5; i++) {
            fullText = hiddenObjectiveParseMultNPC(fullText); //++{Collect-jak-3,MoveTo-NW,Trigger-Book,NPC (1):2, NPC (2):2}
        }

        //Next Dialogue Parsing - FIRE{objective0,...objectiveN}
        fullText = fireParse(fullText);

        //Term Parsing (no print variation) - ADDTERM{item0,item1...itemN}/ADDTERMS{item0,item1...itemN}
        fullText = addTermParse(fullText);

        //Term Parsing (print variation) - ADDTERMPRINT{item0,item1...itemN}/ADDTERMSPRINT{item0,item1...itemN}
        fullText = addTermPrintParse(fullText);

        //Next Dialogue Parsing - GOTO{dialoguePtr}
        fullText = goToParse(fullText,currNPC);

        //Next Dialogue Parsing - INCPTR{npcName}
        fullText = incParse(fullText);

        //Next Dialogue Parsing - GOTOMULT{NPC0:dialoguePTR...NPC1:dialoguePTR}
        fullText = goToMultParse(fullText);

        //Change Type Parsing - CHANGETYPE{WalkType}
        fullText = changeTypeParse(fullText,currNPC);

        //Change Destination Group - CHANGEDEST{destinationPtr}
        fullText = changeDestParse(fullText,currNPC);

        //Change Destination Group for multiple NPCs - CHANGEDESTMULT{NPC0:destinationPtr0,...NPCN:destinationPtrN}
        fullText = changeDestMultParse(fullText);

        //Enable/Disable Obj Event - ACTIVATE{tag0,...tagN} DEACTIVATE{tag0,...tagN} 
        fullText = toggleParse(fullText); //This is intentional
        fullText = toggleParse(fullText);

        //Inventory Event - ADDITEM{item0,count0,item1,count1,...itemN,countN}
        fullText = addParse(fullText);

        //Inventory Event - REMOVEITEM{item0,count0,item1,count1,...itemN,countN}
        fullText = removeParse(fullText);

        //Inventory Event - REMOVEITEM{item0,count0,item1,count1,...itemN,countN}
        fullText = addScoreParse(fullText);

        //Inventory Event - REMOVEITEM{item0,count0,item1,count1,...itemN,countN}
        fullText = checkScoreParse(fullText,currNPC);

        //Change Time Event - TIME{Morning/Afternoon/Evening/Night}
        fullText = timeParse(fullText);

        //Change Scene Event - DESTROY{npc}
        fullText = destroyParse(fullText);

        //Change Scene Event - SCENE{newScene}
        fullText = sceneParse(fullText);

        if(fullText.Trim()==""){
            narrativeData = null;
            return;
        }

        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID).ToList();
        numChoices = choices.Count();

        dialogue.text = "";
        if(numChoices==0){
            dialogue.alignment = TextAlignmentOptions.TopLeft;
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else if(numChoices==1){
            dialogue.alignment = TextAlignmentOptions.TopLeft;
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else{
            dialogue.alignment = TextAlignmentOptions.Top;
            decisionSpace.SetActive(true);
            int copyNumChoices = numChoices;

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
                        if(ColorUtility.TryParseHtmlString(mainColForDialogue,out Color newCol)){
                            decisionGrid[posX,posY].color = newCol;
                        }
                    }
                    copyNumChoices--;
                    pos++;
            }

        }

        isEnabled = true;

        if(decisionGrid!=null){
            currChoiceX = 0;
            currChoiceY = 0;
            if(ColorUtility.TryParseHtmlString(choiceCol,out Color newCol)){
                decisionGrid[0,0].color = newCol;
            }
        }
    }

    public static void scrollX(int i){//Scroll through choices with A and D. Wraparound on overflow.
        if(numChoices<=1){
            return;
        }

        if(ColorUtility.TryParseHtmlString(mainColForDialogue,out Color newCol)){
                decisionGrid[currChoiceY,currChoiceX].color = newCol;
        }

        currChoiceX += i;
        if(currChoiceX<0){ //If less than 0, wraparound
            currChoiceX = decisionGrid.GetLength(1)-1;
            currChoiceY -= 1;
            if(currChoiceY<0){ //Wrapround on Y
                currChoiceY = decisionGrid.GetLength(0)-1;
            }
        }else if(currChoiceX>=decisionGrid.GetLength(1)||decisionGrid[currChoiceY,currChoiceX].enabled==false){ //If greater than size, wraparound
            currChoiceX = 0;
            currChoiceY += 1;
            if(currChoiceY>=decisionGrid.GetLength(0)){ //Wraparound on Y
                currChoiceY = 0;
            }
        }
        while(decisionGrid[currChoiceY,currChoiceX].enabled==false){ //ensure choice is valid
            currChoiceX-=1;
            if(currChoiceX<0){
                currChoiceX = decisionGrid.GetLength(1)-1;
                currChoiceY -= 1;
            }
        }

        if(ColorUtility.TryParseHtmlString(choiceCol,out Color newCol2)){
                decisionGrid[currChoiceY,currChoiceX].color = newCol2;
        }
    }

    public static void scrollY(int i){ //Scroll through choices with W and S. Wraparound on overflow.
        if(numChoices<=1){
            return;
        }

        if(ColorUtility.TryParseHtmlString(mainColForDialogue,out Color newCol)){
                decisionGrid[currChoiceY,currChoiceX].color = newCol;
        }

        currChoiceY += i;
        if(currChoiceY<0){ //Wrapround Y<0
            currChoiceY = decisionGrid.GetLength(0)-1;
        }else if(currChoiceY>=decisionGrid.GetLength(0)||decisionGrid[currChoiceY,currChoiceX].enabled==false){ //Wrapround Y>size
            currChoiceY = 0;
        }
        while(decisionGrid[currChoiceY,currChoiceX].enabled==false){ //Ensure value is valid
            currChoiceY-=1;
        }

        if(ColorUtility.TryParseHtmlString(choiceCol,out Color newCol2)){
                decisionGrid[currChoiceY,currChoiceX].color = newCol2;
        }
    }

    public static void initScript(NpcNavMesh n){ //Initialize dialogue map
        speechComplete = false;
        currNPC = n;
        dialogueContainer = currNPC.getCurrDialogue();
        narrativeData = dialogueContainer.NodeLinks.First(); //Entrypoint node
        nextDialogue(narrativeData.TargetNodeGUID);
    }

    public static bool textComplete(){ //Get if current dialogue line is complete
        return lineComplete;
    }

    public static bool dialogueComplete(){ //Get if dialogue map has terminated
        return speechComplete;
    }

    public static NpcNavMesh.NpcType getNpcType(){ //Get type of NPC in conversation with
        return currNPC.getType();
    }

    private static string objectiveParse(string fullText, NpcNavMesh currNPC){
        if(currNPC == null){
            return fullText;
        }

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
            for(int i = 0; i<questList.Count()-1;i++){ //Add all quests (exclude last position)
                newObjs.Add(Resources.Load<Objective>("Objective System/"+questList[i].Trim()));
            }
            foreach(string s in questList)
                Debug.Log(s);
            ObjectiveDialogueGroup newObjGroup = new ObjectiveDialogueGroup(newObjs,currNPC,int.Parse(questList[questList.Count()-1].Trim()));
            GameObject.Find("Game Controller").GetComponent<GameController>().CreateGrouping(newObjGroup);
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string objectiveParseMultNPC(string fullText){ //ADDQUESTS{q1, q2, q3, npc1:1, npc2:4}
        int startPos = fullText.IndexOf("ADDQUESTS++{");
        int addLen = 12; //Length of ADDQUESTS++{
        if(startPos==-1){
            startPos = fullText.IndexOf("ADDQUEST++{");
            addLen = 11; //Length of ADDQUEST++{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] questList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            List<Objective> newObjs = new List<Objective>();
            List<DialoguePointerMap> npcMap = new List<DialoguePointerMap>();
            for(int i = 0; i<questList.Count();i++){
                if(questList[i].Contains(':')){ //Add NPC and dialogue ptr
                    Debug.Log("Is NPC:Ptr "+questList[i]);
                    string[] npcInfo = questList[i].Trim().Split(':');
                    npcMap.Add(new DialoguePointerMap(npcInfo[0],int.Parse(npcInfo[1])));
                }else{ //Add objectives
                    Debug.Log("Is Objective "+questList[i]);
                    newObjs.Add(Resources.Load<Objective>("Objective System/"+questList[i].Trim()));
                }
            }
            ObjectiveDialogueGroup newObjGroup = new ObjectiveDialogueGroup(newObjs,npcMap);
            GameObject.Find("Game Controller").GetComponent<GameController>().CreateGrouping(newObjGroup);
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

        private static string hiddenObjectiveParse(string fullText, NpcNavMesh currNPC){
        if(currNPC == null){
            return fullText;
        }

        int startPos = fullText.IndexOf("HIDDENQUESTS{");
        int addLen = 13; //Length of HIDDENQUESTS{
        if(startPos==-1){
            startPos = fullText.IndexOf("HIDDENQUEST{");
            addLen = 12; //Length of HIDDENQUEST{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] questList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            List<Objective> newObjs = new List<Objective>();
            for(int i = 0; i<questList.Count()-1;i++){ //Add all quests (exclude last position)
                newObjs.Add(Resources.Load<ObjectiveMisc>("Objective System/"+questList[i].Trim()));
            }
            ObjectiveDialogueGroup newObjGroup = new ObjectiveDialogueGroup(newObjs,currNPC,int.Parse(questList[questList.Count()-1]));
            GameObject.Find("Game Controller").GetComponent<GameController>().CreateHiddenGrouping(newObjGroup);
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string hiddenObjectiveParseMultNPC(string fullText){ //ADDQUESTS{q1, q2, q3, npc1:1, npc2:4}
        int startPos = fullText.IndexOf("HIDDENQUESTS++{");
        int addLen = 15; //Length of HIDDENQUESTS++{
        if(startPos==-1){
            startPos = fullText.IndexOf("HIDDENQUEST++{");
            addLen = 14; //Length of HIDDENQUEST++{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] questList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            List<Objective> newObjs = new List<Objective>();
            List<DialoguePointerMap> npcMap = new List<DialoguePointerMap>();
            for(int i = 0; i<questList.Count();i++){
                if(questList[i].Contains(':')){ //Add NPC and dialogue ptr
                    Debug.Log("Is NPC:Ptr "+questList[i]);
                    string[] npcInfo = questList[i].Trim().Split(':');
                    npcMap.Add(new DialoguePointerMap(npcInfo[0],int.Parse(npcInfo[1])));
                }else{ //Add objectives
                    Debug.Log("Is Objective "+questList[i]);
                    newObjs.Add(Resources.Load<ObjectiveMisc>("Objective System/"+questList[i].Trim()));
                }
            }
            ObjectiveDialogueGroup newObjGroup = new ObjectiveDialogueGroup(newObjs,npcMap);
            GameObject.Find("Game Controller").GetComponent<GameController>().CreateHiddenGrouping(newObjGroup);
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string fireParse(string fullText){
        if(currNPC == null){
            return fullText;
        }

        int startPos = fullText.IndexOf("FIRE{");
            int addLen = 5; //Length of FIRE{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue
                string[] objs = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');

                foreach(string ob in objs){
                    GameObject.Find("Game Controller").GetComponent<GameController>().ForceComplete(Resources.Load<ObjectiveMisc>("Objective System/"+ob.Trim()));
                }

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
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
            string termstoString = ""+itemCol;
            for(int i = 0; i<count;i++){ //Complexity here comes from string manipulation. Add item to known items + add to string
                item = Resources.Load<ItemObject>("Items/"+termList[i].Trim());
                if(count==1){
                    termstoString+=(item.englishName + "(" + item.nativeName + ")");
                }else if(i==0&&count==2){
                    item2 = Resources.Load<ItemObject>("Items/"+termList[i+1].Trim());
                    termstoString+=(item.englishName + "(" + item.nativeName + ") "+ mainCol +"and " + itemCol + item2.englishName + "(" + item2.nativeName + ")");
                    notebook.AddItem(item2);
                }else{
                    if(i==count-2){
                        item2 = Resources.Load<ItemObject>("Items/"+termList[i+1].Trim());
                        termstoString+=(item.englishName + "(" + item.nativeName + ")"+mainCol+", and "+ itemCol + item2.englishName + "(" + item2.nativeName + ")");
                        notebook.AddItem(item2);
                    }else if(i<count-2){
                        termstoString+=(item.englishName + "(" + item.nativeName + ")" + mainCol + ", "+itemCol);
                    }
                }
                notebook.AddItem(item);
            }
            termstoString += mainCol;

            fullText = fullText.Substring(0,startPos) + termstoString  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
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

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            string[] termList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            int count = termList.Count();
            for(int i = 0; i<count;i++){
                if(termList[i]=="THX"){
                    GameObject.Find("First Person Player").GetComponent<UpdateUI>().end();
                    return fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
                }
                Debug.Log(termList[i]);
                notebook.AddItem(Resources.Load<ItemObject>("Items/"+termList[i].Trim()));
            }

            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string goToParse(string fullText, NpcNavMesh currNPC){
        if(currNPC == null){
            return fullText;
        }

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
            return fullText.Trim();
    }

    private static string incParse(string fullText){
        int startPos = fullText.IndexOf("INCPTR{");
        int addLen = 7; //Length of INCPTR{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue
                GameObject.Find(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))).GetComponent<NpcNavMesh>().incDialoguePointer();

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
    }

    private static string goToMultParse(string fullText){
        int startPos = fullText.IndexOf("GOTOMULT{");
            int addLen = 9; //Length of GOTOMULT{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue for multiple NPCs
                string[] gotoList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
                foreach(string str in gotoList){
                    string[] gotoInfo = str.Trim().Split(':');
                    GameObject.Find(gotoInfo[0]).GetComponent<NpcNavMesh>().setDialoguePointer(int.Parse(gotoInfo[1]));
                }

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
    } 

    private static string changeTypeParse(string fullText, NpcNavMesh currNPC){
        if(currNPC == null){
            return fullText;
        }

        int startPos = fullText.IndexOf("CHANGETYPE{");
            int addLen = 11; //Length of CHANGETYPE{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue

                currNPC.setType((NpcNavMesh.NpcType)System.Enum.Parse( typeof(NpcNavMesh.NpcType),fullText.Substring(startPos+addLen,endPos-(startPos+addLen))));
                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
    }   

    private static string changeDestParse(string fullText, NpcNavMesh currNPC){
        if(currNPC == null){
            return fullText;
        }

        int startPos = fullText.IndexOf("CHANGEDEST{");
            int addLen = 11; //Length of CHANGEDEST{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue for multiple NPCs
                currNPC.setDestinationPointer(int.Parse(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))));

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
    } 

        private static string changeDestMultParse(string fullText){
        int startPos = fullText.IndexOf("CHANGEDESTMULT{");
            int addLen = 15; //Length of CHANGEDESTMULT{

            int endPos = -1;

            if(startPos>-1){ //Check for closing bracket
                endPos = fullText.IndexOf("}",startPos+addLen);
            }

            if(endPos>-1){ //If all requirements have passed, set next dialogue for multiple NPCs
                string[] destList = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
                foreach(string str in destList){
                    string[] destInfo = str.Trim().Split(':');
                    GameObject.Find(destInfo[0]).GetComponent<NpcNavMesh>().setDestinationPointer(int.Parse(destInfo[1]));
                }

                fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            }
            return fullText.Trim();
    } 

    private static string toggleParse(string fullText){
        bool toActivate = false;
        int startPos = fullText.IndexOf("DEACTIVATE{");
        int addLen = 11; //Length of DEACTIVATE{
        if(startPos==-1){
            toActivate = true;
            startPos = fullText.IndexOf("ACTIVATE{");
            addLen = 9; //Length of ACTIVATE{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            string[] tags = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            for(int i = 0; i<tags.Count();i++){
                if(toActivate){
                    GameObject.Find("Game Controller").GetComponent<GameController>().ActivateTag(tags[i].Trim());
                }else{
                    GameObject.Find("Game Controller").GetComponent<GameController>().DeactivateTag(tags[i].Trim());
                }
            }

            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string sceneParse(string fullText){
        int startPos = fullText.IndexOf("SCENE{");
        int addLen = 6; //Length of SCENE{

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            sceneSwitch = fullText.Substring(startPos+addLen,endPos-(startPos+addLen));
            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string addParse(string fullText){
        int startPos = fullText.IndexOf("ADDITEMS{");
        int addLen = 9; //Length of ADDITEM{

        if(startPos==-1){
            startPos = fullText.IndexOf("ADDITEM{");
            addLen = 8; //Length of ADDQUEST{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] parts = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            for(int i = 0; i<parts.Count();i+=2){
                int num = int.Parse(parts[i+1]);
                PickupObject item = Resources.Load<PickupObject>("Items/"+parts[i].Trim());
                inventory.AddItem(item,num);
                for (int j = 0 ; j < num ; j++) {
                    objectiveSystem.pickupEvent(item);
                }
            }
            
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string removeParse(string fullText){
        int startPos = fullText.IndexOf("REMOVEITEMS{");
        int addLen = 12; //Length of REMOVEITEMS{

        if(startPos==-1){
            startPos = fullText.IndexOf("REMOVEITEM{");
            addLen = 11; //Length of REMOVEITEM{
        }

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add quests
            string[] parts = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            for(int i = 0; i<parts.Count();i+=2){
                inventory.RemoveItem(Resources.Load<ItemObject>("Items/"+parts[i].Trim()),int.Parse(parts[i+1]));
            }
            
            fullText = fullText.Substring(0,startPos) + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string destroyParse(string fullText){
        int startPos = fullText.IndexOf("DESTROY{");
        int addLen = 8; //Length of DESTROY{

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            Destroy(GameObject.Find(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))));
            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string timeParse(string fullText){
        int startPos = fullText.IndexOf("TIME{");
        int addLen = 5; //Length of TIME{

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            if(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))=="Morning"){
               timeChange = 0;
            }else if(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))=="Afternoon"){
                timeChange = 1;
            }else if(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))=="Evening"){
                timeChange = 2;
            }else if(fullText.Substring(startPos+addLen,endPos-(startPos+addLen))=="Night"){
                timeChange = 3;
            }
            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string addScoreParse(string fullText){
        int startPos = fullText.IndexOf("ADDPOINT{");
        int addLen = 9; //Length of ADDPOINT{

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            score+=float.Parse(fullText.Substring(startPos+addLen,endPos-(startPos+addLen)));
            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
        }
        return fullText.Trim();
    }

    private static string checkScoreParse(string fullText, NpcNavMesh currNpc){
        int startPos = fullText.IndexOf("CHECKSCORE{");
        int addLen = 11; //Length of CHECKSCORE{

        int endPos = -1;

        if(startPos>-1){ //Check for closing bracket
            endPos = fullText.IndexOf("}",startPos+addLen);
        }

        if(endPos>-1){ //If all requirements have passed, add items to notebook (no string replacement/manipulation)
            string[] parts = fullText.Substring(startPos+addLen,endPos-(startPos+addLen)).Split(',');
            float playerPercent = score/float.Parse(parts[3].Trim());
            float passingPercent = float.Parse(parts[2].Trim())/100.0f;
            if(playerPercent>passingPercent){
                currNPC.setDialoguePointer(int.Parse(parts[0].Trim()));
            }else{
                currNPC.setDialoguePointer(int.Parse(parts[1].Trim()));
            }

            fullText = fullText.Substring(0,startPos)  + fullText.Substring(endPos+1,fullText.Length-endPos-1);
            score = 0.0f;
        }
        return fullText.Trim();
    }
}
