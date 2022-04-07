using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destination Object", menuName = "Item System/Items/Destination")]
public class DestinationObject : ItemObject
{
    public void Awake() {
        type = ItemType.Trigger;
    }
}
