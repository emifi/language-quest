using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookUI : MonoBehaviour
{
    public NotebookObject notebook;
    public Canvas notebookUI;
    public static GameObject dictUI;
    public static GameObject dictButtons; //Button space
    private Button[] buttons;
    private Button homeButton;
    private static GameObject leftButton;
    private static GameObject rightButton;
    private TMP_Text page1;
    private TMP_Text page2;
    private static bool atHome; //On "home" dictionary page
    private static char currPage; //Current page (in ASCII)
    private static int posPtr; //Further page pointer for ASCII pages that overflow
    private static float timeout; //Prevents double page turns
    private string str1; //Text for page1
    private string str2; //Text for page2

    void Start() {
        notebookUI.enabled = false;
        atHome = true;
        currPage = ' ';
        posPtr = 0;
        str1 = "";
        str2 = "";
        dictUI = GameObject.Find("DictLetters");
        page1 = GameObject.Find("Canv/Frame/Page1/Entries/Text").GetComponent<TMP_Text>();
        page2 = GameObject.Find("Canv/Frame/Page2/Entries/Text").GetComponent<TMP_Text>();
        buttons = dictUI.GetComponentsInChildren<Button>();
        dictButtons = GameObject.Find("DictButtonFrame");
        homeButton = GameObject.Find("DictHomeButton").GetComponent<Button>();
        leftButton = GameObject.Find("DictLeftButton");
        rightButton = GameObject.Find("DictRightButton");
        timeout = 0;

        foreach(Button btn in buttons){ //While this IS flexible, additional buttons
            if(btn.name.Length==1){     //will have to continue the ASCII sequence so that the array can remain squential
                btn.onClick.AddListener(() => navToChapter(btn.name[0])); //Please note new logic will need to be made in the dictionary
            }
        }
        homeButton.onClick.AddListener(() => navHome());
        leftButton.GetComponent<Button>().onClick.AddListener(() => turnPage(-6)); //Dictionary pages are hard-coded to have 6 pages
        rightButton.GetComponent<Button>().onClick.AddListener(() => turnPage(6)); //This always disallows overflow.
    }

    // Update is called once per frame
    void Update()
    {
        str1 = "";
        str2 = "";

        if(atHome){ //If at home, get the 3 newest definitions
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
        }else{ //Get posPtr to posPtr+5 definitions on page. If there are more pages, display buttons
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

                if(posPtr+6<(dictionaryEntry.Count)){
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

    public static void navToChapter(char c){ //Nav to chapter of the corresponding ASCII
        currPage = c;
        atHome = false;
        dictUI.SetActive(false);
        dictButtons.SetActive(true);
    }

    public static void navHome(){ //Return to homepage
        atHome = true;
        dictUI.SetActive(true);
        dictButtons.SetActive(false);
    }

    public static void turnPage(int input){ //Turn page within chapter
        if(Time.time-timeout>0.25f){//prevents double-calls
            timeout = Time.time;
            posPtr += input;
        }
    }
}
