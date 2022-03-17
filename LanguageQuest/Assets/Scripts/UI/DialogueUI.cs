using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Canvas dialogueUI;
    private Text dialogue;
    private static NpcNavMesh currNPC;
    private static int posPtr;
    private static int textPos;
    private static string[] lines; 
    private float textDelay;
    private static string str;
    public static bool lineComplete = false;
    public static bool speechComplete = true;
    // Start is called before the first frame update
    void Start()
    {
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueUI.enabled = false;
        posPtr = 0;
        textPos = 0;
        str = "";
        lines = null;
        dialogue = dialogueUI.transform.Find("DialogueBox/Paper/Text").GetComponent<Text>();
        textDelay = .15f;
    }

    // Update is called once per frame
    void Update()
    {
        if(lines==null||posPtr>=lines.Length){
            speechComplete = true;
            dialogueUI.enabled = false;
            return;
        }
        if(Time.time-textDelay>.05f&&!speechComplete&&textPos<lines[posPtr].Length){
            textDelay = Time.time;
            str+=lines[posPtr][textPos];
            textPos++;
        }
        if(textPos>=lines[posPtr].Length){
            lineComplete = true;
        }

        if(currNPC!=null&&currNPC.getType()!=NpcNavMesh.NpcType.Proximity&&currNPC.getType()!=NpcNavMesh.NpcType.Stationary){
            currNPC.stopMovement();
        }
        
        dialogue.text = str;
    }

    public static void turnPage(){
        lineComplete = false;
        str = "";
        posPtr++;
        textPos=0;
    }

    public static void setScript(string[] script, NpcNavMesh n){
        speechComplete = false;
        lineComplete = false;
        posPtr = 0;
        lines = script;
        currNPC = n;
    }

    public static bool textComplete(){
        return lineComplete;
    }

    public static bool dialogueComplete(){
        return speechComplete;
    }

}
