using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "QuestID-MoveTo-Destination", menuName = "Objective System/Objectives/MoveTo")]
public class ObjectiveMoveTo : Objective {
    [Header("MoveTo-type Paramaters")]
    public DestinationObject destination;

    public void Awake() {
        type = ObjectiveType.MoveTo;
    }
}