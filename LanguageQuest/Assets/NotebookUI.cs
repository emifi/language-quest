using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    public NotebookObject notebook;
    public Canvas notebookUI;

    void Start() {
        notebookUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        string str = "Notebook Protoype: \n";

        foreach (NotebookSlot slot in notebook.container) {
            ItemObject item = slot.item;
            str += $"(image){item.nativeName}/{item.englishName}: {item.description}\n";
        }
        notebookUI.transform.Find("Image/Text").GetComponent<Text>().text = str;
    }
}
