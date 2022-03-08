using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType{
    Pickup,
    Entry,
    Trigger,
    Default,
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public GameObject image;
    public ItemType type;
    public string displayName;
    [TextArea(15,20)]
    public string description;
    public string interactionString = "to interact";
}
