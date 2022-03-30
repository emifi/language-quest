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
    private Text dialogue;
    private static GameObject decisionSpace; //The physical UI space
    private static Text decisionA;
    private static Text decisionB;
    private static Text decisionC;
    private static Text decisionD;
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

    private static Text[,] decisionGrid; // Array representation of dialogue space
    private static int numChoices; //Used to hold count of choices
    private static int currChoiceX; //Used for highlight/selection of choices
    private static int currChoiceY;

    // Start is called before the first frame update
    void Start()
    {
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueUI.enabled = false;
        textPos = 0;
        displayedText = "";
        narrativeData = null;
        choices = null;
        fullText = null;
        dialogue = dialogueUI.transform.Find("DialogueBox/Paper/Text").GetComponent<Text>();
        decisionSpace = GameObject.Find("SelectionSpace");
        textDelay = .15f;
    }

    // Update is called once per frame
    void Update()
    {
        if(narrativeData==null||fullText==null){
            lineComplete = false;
            speechComplete = true;
            narrativeData = null;
            choices = null;
            fullText = null;
            dialogueUI.enabled = false;
            displayedText = "";
            textPos = 0;
            return;
        }
        if(Time.time-textDelay>.05f&&!speechComplete&&textPos<fullText.Length){
            textDelay = Time.time;
            displayedText+=fullText[textPos];
            textPos++;
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
        decisionGrid[currChoiceY,currChoiceX].color = Color.black;
        nextDialogue(choices[(currChoiceY*decisionGrid.GetLength(1))+currChoiceX].TargetNodeGUID);
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
        fullText = fullText.Replace("{replacename}",currNPC.name);


        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID).ToList();
        numChoices = choices.Count();

        if(numChoices==0){
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else if(numChoices==1){
            decisionSpace.SetActive(false);
            decisionGrid = null;
        }else{
            decisionSpace.SetActive(true);
            int copyNumChoices = numChoices;

            decisionGrid = new Text[2,2]; //Feel free to resize decision grid based on your maximum # of responses. 
                                      //Methods will react to this without issue
            decisionGrid[0,0] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row1/A").GetComponent<Text>();
            decisionGrid[0,1] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row1/B").GetComponent<Text>();
            decisionGrid[1,0] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row2/C").GetComponent<Text>();
            decisionGrid[1,1] = GameObject.Find("DialogueBox/Paper/SelectionSpace/Row2/D").GetComponent<Text>();
            
            int pos = 0;
            foreach(Text t in decisionGrid){
                    if(copyNumChoices<=0){
                        t.enabled = false;
                    }else{
                        int posX = pos%decisionGrid.GetLength(1);
                        int posY = pos/decisionGrid.GetLength(1);
                        decisionGrid[posY,posX].text = choices[pos].PortName;
                    }
                    copyNumChoices--;
                    pos++;
            }

        }

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
        decisionGrid[currChoiceY,currChoiceX].color = Color.black;
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
        decisionGrid[currChoiceY,currChoiceX].color = Color.black;
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

    public static void setScript(NpcNavMesh n){
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

}
