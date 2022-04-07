using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "New MoveTo Objective", menuName = "Objective System/Objectives/MoveTo")]
public class ObjectiveMoveTo : Objective {
    [Header("MoveTo-type Paramaters")]
    public DestinationObject destination;

    public void Awake() {
        type = ObjectiveType.MoveTo;
    }
}