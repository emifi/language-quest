using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class Objective : ScriptableObject
{
    public ObjectiveType type;
    public string objectiveString;
    [System.NonSerialized]
    public ObjectiveStatus status;
    [HideInInspector]
    public GameObject panelUI;
}

public enum ObjectiveStatus {
    Incomplete,
    Complete,
}

public enum ObjectiveType {
    Collect,
    MoveTo,
    Trigger,
    TalkTo,
    Follow,
    Misc,
}
