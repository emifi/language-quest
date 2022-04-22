using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "QuestID-Misc", menuName = "Objective System/Objectives/Misc")]
public class ObjectiveMisc : Objective {

    public void Awake() {
        type = ObjectiveType.Misc;
    }
}

