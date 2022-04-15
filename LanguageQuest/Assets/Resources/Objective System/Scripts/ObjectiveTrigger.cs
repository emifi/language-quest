using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "New Trigger Objective", menuName = "Objective System/Objectives/Trigger")]
public class ObjectiveTrigger : Objective {
    [Header("Trigger-type Paramaters")]
    public TriggerObject trigger;

    public void Awake() {
        type = ObjectiveType.Trigger;
    }
}