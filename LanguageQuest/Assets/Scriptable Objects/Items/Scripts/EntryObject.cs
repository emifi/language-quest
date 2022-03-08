using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entry Object", menuName = "Item System/Items/Entry")]
public class EntryObject : ItemObject
{
    public void Awake() {
        type = ItemType.Entry;
    }
}
