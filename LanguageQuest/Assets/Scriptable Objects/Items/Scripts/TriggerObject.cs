using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trigger Object", menuName = "Item System/Items/Trigger")]
public class TriggerObject : ItemObject
{
    public void Awake() {
        type = ItemType.Trigger;
    }
}