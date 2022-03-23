using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    public ObjectiveType type;
    public string objectiveString;
    public ObjectiveStatus status;
    [Header("Collect-type Paramaters")]
    public PickupObject item;
    public int current;
    public int required;
    [Header("MoveTo-type Paramaters")]
    public GameObject destination;
    [Header("Trigger-type Paramaters")]
    public TriggerObject trigger;

    public Objective(ObjectiveType type, string objectiveString, PickupObject item, int current, int required) {
        this.type = type;
        this.objectiveString = objectiveString;
        this.status = ObjectiveStatus.Incomplete;
        this.item = item;
        this.current = current;
        this.required = required;

        if (current >= required) {
            this.status = ObjectiveStatus.Complete;
            this.current = required;
        }
    }

    public Objective(ObjectiveType type, string objectiveString, GameObject destination) {
        this.type = type;
        this.objectiveString = objectiveString;
        this.status = ObjectiveStatus.Incomplete;
        this.destination = destination;
    }

    public Objective(ObjectiveType type, string objectiveString, TriggerObject trigger) {
        this.type = type;
        this.objectiveString = objectiveString;
        this.status = ObjectiveStatus.Incomplete;
        this.trigger = trigger;
    }
}

public enum ObjectiveStatus {
    Incomplete,
    Complete,
}

public enum ObjectiveType {
    Collect,
    MoveTo,
    Trigger,
}
