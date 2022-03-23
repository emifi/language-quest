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
    private static GameObject decisionSpace;
    private static DialogueContainer dialogueContainer;
    private static NpcNavMesh currNPC;
    private static int textPos;
    private float textDelay;
    private static string displayedText;
    public static bool lineComplete = false;
    public static bool speechComplete = true;

    private static NodeLinkData narrativeData;
    private static IEnumerable<NodeLinkData> choices;
    private static string fullText;

    private static int numChoices;
    private static int currChoice;

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
        foreach(var choice in choices){
            nextDialogue(choice.TargetNodeGUID);
            return;
        }
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
        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        numChoices = choices.Count();
        Debug.Log("S + " + numChoices);
        if(numChoices==0){
            decisionSpace.SetActive(false);
        }else if(numChoices==1){
            decisionSpace.SetActive(false);
        }else{
            decisionSpace.SetActive(true);
        }
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

}
