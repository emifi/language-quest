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
    private static Button[] buttons;
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
    private int entriesPerPage; //Number of entries per page

    void Start() {
        //DataStructs.populateGlobalDictionary();
        notebook = DataStructs.notebook;
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
        entriesPerPage = 2;


        int i = 0;
        foreach(Button btn in buttons){ //While this IS flexible, any additional buttons 0th character
            if(btn.name.Length==1){     //will have to continue the ASCII sequence so that the array can remain squential 
                                        //- buttons can be renamed after this point
                btn.onClick.AddListener(() => navToChapter(btn.name[0])); //Please note new logic will also need to be made in the dictionary
                if(notebook.dictionary[i].chap.Count==0){
                    btn.interactable = false;
                }else{
                    btn.interactable = true;
                }
            }
            i++;
        }
        homeButton.onClick.AddListener(() => navHome());
        leftButton.GetComponent<Button>().onClick.AddListener(() => turnPage(-(2*entriesPerPage))); //Dictionary pages are hard-coded to have 6 pages
        rightButton.GetComponent<Button>().onClick.AddListener(() => turnPage(2*entriesPerPage)); //This always disallows overflow.
    }

    // Update is called once per frame
    void Update()
    {
        str1 = "";
        str2 = "";

        if(atHome){ //If at home, get the 3 newest definitions
            str1+= $"<b>Most Recent Entries:</b> \n";
            str2+= $"<b>Table of Contents:</b> \n";
            int src = entriesPerPage;
            if(notebook.container.Count<src){
                src = notebook.container.Count;
            }
            for(int i = 1; i<=src;i++){
                ItemObject item = notebook.container[notebook.container.Count-i].item;
                str1 += $"<size=250%><sprite name=\"{item.imageSprite}\"></size> {item.nativeName}/{item.englishName}: {item.description}\n";
            }
        }else{ //Get posPtr to posPtr+(n-1) definitions on page. If there are more pages, display buttons
            str1+= $"<b>Words that start with \"{currPage}\":</b> \n";
            List<NotebookSlot> dictionaryEntry = notebook.dictionary[NotebookObject.mapper(currPage)].chap;
            if(currPage>=65&&currPage<=90&&dictionaryEntry!=null){
                for(int i = 0; i<(2*entriesPerPage);i++){
                    if(posPtr + i < dictionaryEntry.Count){
                        ItemObject item = dictionaryEntry[posPtr+i].item;
                        if(i<entriesPerPage){
                            str1 += $"<size=250%><sprite name=\"{item.imageSprite}\"></size> {item.nativeName}/{item.englishName}: {item.description}\n";
                        }else{
                            str2 += $"<size=250%><sprite name=\"{item.imageSprite}\"></size> {item.nativeName}/{item.englishName}: {item.description}\n";
                        }
                    }
                }

                if(posPtr!=0){
                    leftButton.SetActive(true);
                }else{
                    leftButton.SetActive(false);
                }

                if(posPtr+(2*entriesPerPage)<(dictionaryEntry.Count)){
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

    public static void enableCharButton(int c){
        if(!buttons[c].interactable){
            buttons[c].interactable = true;
        }
    }

    public static void navHome(){ //Return to homepage
        posPtr = 0;
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