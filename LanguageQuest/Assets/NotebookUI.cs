using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    public NotebookObject notebook;
    public Canvas notebookUI;
    private Text page1;
    private Text page2;

    void Start() {
        notebookUI.enabled = false;
        page1 = notebookUI.transform.Find("Frame/Page1/Entries/Text").GetComponent<Text>();
        page2 = notebookUI.transform.Find("Frame/Page2/Entries/Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string str = "Notebook Protoype: \n";
        string str2 = "Subset:\n";

        if(notebook.dictionary[NotebookObject.mapper('b')]!=null){
            foreach (NotebookSlot slot in notebook.dictionary[NotebookObject.mapper('b')]) {
            ItemObject item = slot.item;
            str2 += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
            }
        }

        foreach (NotebookSlot slot in notebook.container) {
            ItemObject item = slot.item;
            str += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
        }
        page1.text = str;
        page2.text = str2;
    }
}
