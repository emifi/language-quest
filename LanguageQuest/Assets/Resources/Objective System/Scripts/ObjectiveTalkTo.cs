using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "New TalkTo Objective", menuName = "Objective System/Objectives/TalkTo")]
public class ObjectiveTalkTo : Objective {
    [Header("TalkTo-type Paramaters")]
    public NPCIdentifierObject npcID;

    public void Awake() {
        type = ObjectiveType.TalkTo;
    }
}