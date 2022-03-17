using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    public NotebookObject notebook;
    public Canvas notebookUI;
    public static GameObject dictUI;
    public static GameObject dictButtons;
    public Button[] buttons;
    public Button homeButton;
    public static GameObject leftButton;
    public static GameObject rightButton;
    private Text page1;
    private Text page2;
    private static bool atHome;
    private static char currPage;
    private static int posPtr;
    private static float timeout;
    private string str1;
    private string str2;

    void Start() {
        notebookUI.enabled = false;
        atHome = true;
        currPage = ' ';
        posPtr = 0;
        str1 = "";
        str2 = "";
        dictUI = GameObject.Find("DictLetters");
        page1 = notebookUI.transform.Find("Canv/Frame/Page1/Entries/Text").GetComponent<Text>();
        page2 = notebookUI.transform.Find("Canv/Frame/Page2/Entries/Text").GetComponent<Text>();
        buttons = dictUI.GetComponentsInChildren<Button>();
        dictButtons = GameObject.Find("DictButtonFrame");
        homeButton = GameObject.Find("DictHomeButton").GetComponent<Button>();
        leftButton = GameObject.Find("DictLeftButton");
        rightButton = GameObject.Find("DictRightButton");
        timeout = 0;

        foreach(Button btn in buttons){
            if(btn.name.Length==1){
                btn.onClick.AddListener(() => navToPage(btn.name[0]));
            }
        }
        homeButton.onClick.AddListener(() => navHome());
        leftButton.GetComponent<Button>().onClick.AddListener(() => turnPage(-6));
        rightButton.GetComponent<Button>().onClick.AddListener(() => turnPage(6));
    }

    // Update is called once per frame
    void Update()
    {
        str1 = "";
        str2 = "";

        if(atHome){
            str1+= $"Most Recent Entries: \n";
            str2+= $"Table of Contents: \n";
            int src = 3;
            if(notebook.container.Count<src){
                src = notebook.container.Count;
            }
            for(int i = 1; i<=src;i++){
                ItemObject item = notebook.container[notebook.container.Count-i].item;
                str1 += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
            }
        }else{
            str1+= $"Words that start with \"{currPage}\": \n";
            List<NotebookSlot> dictionaryEntry = notebook.dictionary[NotebookObject.mapper(currPage)];
            if(currPage>=65&&currPage<=90&&dictionaryEntry!=null){
                for(int i = 0; i<6;i++){
                    if(posPtr + i < dictionaryEntry.Count){
                        ItemObject item = dictionaryEntry[posPtr+i].item;
                        if(i<3){
                            str1 += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
                        }else{
                            str2 += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
                        }
                    }
                }

                if(posPtr!=0){
                    leftButton.SetActive(true);
                }else{
                    leftButton.SetActive(false);
                }

                if(posPtr+6<=(dictionaryEntry.Count)){
                    rightButton.SetActive(true);
                }else{
                    rightButton.SetActive(false);
                }
            }

            if(dictionaryEntry==null){
                    rightButton.SetActive(false);
                    leftButton.SetActive(false);
            }
        }

        
        page1.text = str1;
        page2.text = str2;
    }

    public static void navToPage(char c){
        currPage = c;
        atHome = false;
        dictUI.SetActive(false);
        dictButtons.SetActive(true);
    }

    public static void navHome(){
        atHome = true;
        dictUI.SetActive(true);
        dictButtons.SetActive(false);
    }

    public static void turnPage(int input){
        if(Time.time-timeout>0.25f){//prevents double-calls
            timeout = Time.time;
            posPtr += input;
        }
    }
}
