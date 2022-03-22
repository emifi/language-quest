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
        textDelay = .15f;
    }

    // Update is called once per frame
    void Update()
    {
        if(narrativeData==null||fullText==null){
            speechComplete = true;
            dialogueUI.enabled = false;
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
        foreach(var choice in choices){
            nextDialogue(choice.TargetNodeGUID);
            return;
        }
    }

    private static void nextDialogue(string narrativeDataGUID){
        if(narrativeDataGUID==null){
            lineComplete = false;
            speechComplete = true;
            narrativeData = null;
            choices = null;
            fullText = null;
            return;
        }
        lineComplete = false;
        displayedText = "";
        textPos=0;
        fullText = dialogueContainer.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeData.TargetNodeGUID);
    }

    public static void setScript(NpcNavMesh n){
        speechComplete = false;
        lineComplete = false;
        currNPC = n;
        dialogueContainer = currNPC.getCurrDialogue();
        narrativeData = dialogueContainer.NodeLinks.First(); //Entrypoint node
        fullText = dialogueContainer.DialogueNodeData.Find(x => x.NodeGUID == narrativeData.TargetNodeGUID).DialogueText;
        choices = dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == narrativeData.TargetNodeGUID);
    }

    public static bool textComplete(){
        return lineComplete;
    }

    public static bool dialogueComplete(){
        return speechComplete;
    }

}
