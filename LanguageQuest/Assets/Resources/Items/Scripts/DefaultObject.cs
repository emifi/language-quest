using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Default Object", menuName = "Item System/Items/Defaut")]
public class DefaultObject : ItemObject
{
    public void Awake() {
        type = ItemType.Default;
    }
}
