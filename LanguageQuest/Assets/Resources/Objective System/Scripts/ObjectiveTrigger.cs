using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "QuestID-Trigger-TriggerName", menuName = "Objective System/Objectives/Trigger")]
public class ObjectiveTrigger : Objective {
    [Header("Trigger-type Paramaters")]
    public TriggerObject trigger;

    public void Awake() {
        type = ObjectiveType.Trigger;
    }
}