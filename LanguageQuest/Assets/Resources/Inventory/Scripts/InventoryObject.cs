using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Item System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public int maxSize = 5;
    public List<InventorySlot> container = new List<InventorySlot>();

    public bool AddItem(ItemObject item) {
        return AddItem(item, 1);
    }

    public bool AddItem(ItemObject item, int amount) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                GameObject.Find("First Person Player").GetComponent<UpdateUI>().AddInvDisplay(item,amount);
                container[i].AddAmount(amount);
                return true;
            }
        }
        if (container.Count < maxSize) {
            GameObject.Find("First Person Player").GetComponent<UpdateUI>().AddInvDisplay(item,amount);
            container.Add(new InventorySlot(item, amount));
            return true;
        } else {
            GameObject.Find("First Person Player").GetComponent<UpdateUI>().AddInvDisplay(item,0);
            return false;
        }
    }

    public bool RemoveItem(ItemObject item) {
        return RemoveItem(item, 1);
    }

    public bool RemoveItem(ItemObject item, int amount) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                if (container[i].amount < amount){ 
                    GameObject.Find("First Person Player").GetComponent<UpdateUI>().RemoveFailDisplay(item,amount);
                    return false;
                }
                container[i].AddAmount(-amount);
                GameObject.Find("First Person Player").GetComponent<UpdateUI>().RemoveInvDisplay(item,amount);
                if (container[i].amount == 0) {
                    container.RemoveAt(i);
                }
                return true;
            }
        }
        GameObject.Find("First Person Player").GetComponent<UpdateUI>().RemoveFailDisplay(item,amount);
        return false;
    }
    public bool HasItem(ItemObject item) {
        return HasItem(item, 1);
    }
    public bool HasItem(ItemObject item, int amount) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item && container[i].amount >= amount) {
                return true;
            }
        }
        return false;
    }

    public int GetItem(ItemObject item) {
        for (int i = 0; i < container.Count; i++) {
            if (item == container[i].item) {
                return container[i].amount;
            }
        }
        return 0;
    }
}

[System.Serializable]
public class InventorySlot 
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject newItem, int newAmount) {
        this.item = newItem;
        this.amount = newAmount;
    }

    public void AddAmount(int value) {
        amount += value;
    }
}

