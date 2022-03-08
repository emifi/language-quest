using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Notebook", menuName = "Item System/Notebook")]
public class NotebookObject : ScriptableObject
{
    public List<NotebookSlot> container = new List<NotebookSlot>();

    public async void AddItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                return;
            }
        }
        container.Add(new NotebookSlot(item));
    }

    public void RemoveItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                container.RemoveAt(i);
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
}

[System.Serializable]
public class NotebookSlot 
{
    public ItemObject item;

    public NotebookSlot(ItemObject newItem) {
        this.item = newItem;
    }
}

