using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CreateAssetMenu(fileName = "QuestID-TalkTo-NPCName", menuName = "Objective System/Objectives/TalkTo")]
public class ObjectiveTalkTo : Objective {
    [Header("TalkTo-type Paramaters")]
    public NPCIdentifierObject npcID;

    public void Awake() {
        type = ObjectiveType.TalkTo;
    }
}