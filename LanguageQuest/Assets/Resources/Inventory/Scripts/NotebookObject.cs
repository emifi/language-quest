using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Notebook", menuName = "Item System/Notebook")]
public class NotebookObject : ScriptableObject
{
    public List<NotebookSlot> container = new List<NotebookSlot>();
    public ChapterSlot[] dictionary = new ChapterSlot[37];//A-Z + 0-9 + Misc. Space
    public GameObject objectiveUI;
    public GameObject panelUI;

    public void AddItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                return;
            }
        }
        container.Add(new NotebookSlot(item));
        GameObject.Find("First Person Player").GetComponent<UpdateUI>().AddNotebookDisplay(item);
        if(dictionary[mapper(item.englishName[0])].chap.Count==0){
            NotebookUI.enableCharButton(mapper(item.englishName[0]));
            Debug.Log("ADDING");
            dictionary[mapper(item.englishName[0])].chap.Add(new NotebookSlot(item));
            Debug.Log(dictionary[mapper(item.englishName[0])].chap.Count);
            return;
        }

        
        for (int i = 0; i < dictionary[mapper(item.englishName[0])].chap.Count; i++) {
            if (System.String.Compare(item.englishName,dictionary[mapper(item.englishName[0])].chap[i].item.englishName)<0) {
                dictionary[mapper(item.englishName[0])].chap.Insert(i,new NotebookSlot(item));
                return;
            }
        }
        dictionary[mapper(item.englishName[0])].chap.Add(new NotebookSlot(item)); //this means that the word will be added to the end of the segment
    }

    public void RemoveItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                container.RemoveAt(i);
            }
        }

        List<NotebookSlot> nList = dictionary[mapper(item.name[0])].chap;
        for (int i = 0; i < nList.Count; i++) {
            if (item == nList[i].item) {
                nList.RemoveAt(i);
            }
        }
    }

    public bool HasItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                return true;
            }
        }
        return false;
    }

    public static int mapper(char c){ //maps char to dictionary position
        if(c>=48&&c<=57){
            return c-48+26;
        }else if(c>=65&&c<=90){
            return c - 65;
        }else if(c>=97&&c<=122){
            return c - 97;
        }
        return 36; //should never be reached
    }

    public void debugger(int x){
        Debug.Log("SRC: " + x);
        for(int i = 0; i<dictionary.Length;i++){
                Debug.Log("LETTER: "+((char)(i+65)));
                if(dictionary[i]!=null){
                    foreach(NotebookSlot thing in dictionary[i].chap){
                    Debug.Log(thing.item.englishName);
                    }
                }
            }
    }

}

[System.Serializable]
public class NotebookSlot 
{
    public ItemObject item;

    public NotebookSlot(ItemObject newItem) {
        this.item = newItem;
    }
}

[System.Serializable]
public class ChapterSlot 
{
    public List<NotebookSlot> chap;

    public ChapterSlot(List<NotebookSlot> newChap) {
        this.chap = newChap;
    }
}