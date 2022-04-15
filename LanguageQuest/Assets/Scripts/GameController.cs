using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<ObjectiveDialogueGroup> objectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    ObjectiveSystem objectiveSystem;
    static int group_num = 0;
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
                objectiveSystem.removeCompletedObjectives(odg.objectives);
                groupsToRemove.Add(odg);
                if (odg.finalObjective != null) {
                    //objectiveSystem.addObjective(odg.finalObjective);
                }
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
        group_num += 1;
        ObjectiveDialogueGroup group = new ObjectiveDialogueGroup(objectives, npc, pointer);
        objectiveSystem.addObjectiveList(objectives, group_num);
        objectiveDialogueGroups.Add(group);
    }

    public void CreateGrouping(ObjectiveDialogueGroup group) {
        group_num += 1;
        objectiveSystem.addObjectiveList(group.objectives, group_num);
        objectiveDialogueGroups.Add(group);
    }
}

// This class is used to pair objectives into groupings so that they can all be checked for completion. Upon completion, their quest-giver npc will have their dialogue updated to reflect
[System.Serializable]
public class ObjectiveDialogueGroup
{
    public List<Objective> objectives = new List<Objective>();
    public Objective finalObjective;
    public List<DialoguePointerMap> ptrMap = new List<DialoguePointerMap>();

    // Construct a group from a single npc ptr pair
    public ObjectiveDialogueGroup(List<Objective> objectives, NpcNavMesh npc, int pointer, Objective final = null) {
        this.objectives = objectives;
        this.finalObjective = final;
        this.ptrMap.Add(new DialoguePointerMap(npc, pointer));
    }

    // Construct a group from a list of DialoguePointerMap
    public ObjectiveDialogueGroup(List<Objective> objectives, List<DialoguePointerMap> map, Objective final = null) {
        this.objectives = objectives;
        this.finalObjective = final;
        this.ptrMap = map;
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
        foreach (DialoguePointerMap pair in ptrMap)
            pair.npc.setDialoguePointer(pair.ptr);
    }
}

public class DialoguePointerMap {
    public NpcNavMesh npc;
    public int ptr;

    public DialoguePointerMap(NpcNavMesh npc, int ptr) {
        this.npc = npc;
        this.ptr = ptr;
    }

    // Takes a raw string and locates the NPC 
    public DialoguePointerMap(string npcStr, int ptr) {
        this.npc = GameObject.Find(npcStr).GetComponent<NpcNavMesh>();
        this.ptr = ptr;
    }
}