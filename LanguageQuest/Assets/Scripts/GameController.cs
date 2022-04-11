using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<ObjectiveDialogueGroup> objectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    ObjectiveSystem objectiveSystem;
    // Start is called before the first frame update
    void Start()
    {
        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        List<ObjectiveDialogueGroup> groupsToRemove = new List<ObjectiveDialogueGroup>();
        foreach (ObjectiveDialogueGroup odg in objectiveDialogueGroups) {
            if (odg.CheckForCompletion()) {
                odg.PushDialoguePointer();
                groupsToRemove.Add(odg);
            }
        }
        foreach (ObjectiveDialogueGroup odg in groupsToRemove) {
            objectiveDialogueGroups.Remove(odg);
        }
    }

    // Can use to check if there is an active ObjectiveDialogue grouping prior to inserting a new one
    public bool HasActiveGrouping() {
        if (objectiveDialogueGroups.Count > 0) {
            return true;
        }
        return false;
    }

    // Creates a new ObjDiagGroup, sends it to the objective system and adds it to the GameController container
    public void CreateGrouping(List<Objective> objectives, NpcNavMesh npc, int pointer) {
        ObjectiveDialogueGroup group = new ObjectiveDialogueGroup(objectives, npc, pointer);
        objectiveSystem.addObjectiveList(objectives);
        objectiveDialogueGroups.Add(group);
    }

    public void CreateGrouping(ObjectiveDialogueGroup group) {
        objectiveSystem.addObjectiveList(group.objectives);
        objectiveDialogueGroups.Add(group);
    }
}

// This class is used to pair objectives into groupings so that they can all be checked for completion. Upon completion, their quest-giver npc will have their dialogue updated to reflect
[System.Serializable]
public class ObjectiveDialogueGroup
{
    public List<Objective> objectives = new List<Objective>();
    public NpcNavMesh npc;
    public int dialoguePointer;

    public ObjectiveDialogueGroup(List<Objective> objectives, NpcNavMesh npc, int pointer) {
        this.objectives = objectives;
        this.npc = npc;
        this.dialoguePointer = pointer;
    }

    public bool CheckForCompletion() {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Incomplete) {
                return false;
            }
        }
        return true;
    }

    public void PushDialoguePointer() {
        npc.setDialoguePointer(this.dialoguePointer);
    }
}