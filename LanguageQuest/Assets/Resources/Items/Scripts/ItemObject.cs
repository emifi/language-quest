using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType{
    Pickup,
    Entry,
    Trigger,
    Destination,
    Default,
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public GameObject image;
    public ItemType type;
    public bool canBeAddedToNotebook;
    public string nativeName;
    public string englishName;
    [TextArea(15,20)]
    public string description;
    public string interactionString = "to interact";
}
