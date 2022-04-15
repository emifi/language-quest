using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "New Collect Objective", menuName = "Objective System/Objectives/Collect")]
public class ObjectiveCollect : Objective {
    [Header("Collect-type Paramaters")]
    public PickupObject item;
    [System.NonSerialized]
    public int current;
    public int required;

    public void Awake() {
        type = ObjectiveType.Collect;
    }
}

