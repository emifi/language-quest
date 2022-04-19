using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<ObjectiveDialogueGroup> objectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    [Header("These groups will not be displayed on the players UI. Useful in non-linear progression for checking if objectives from other groups have been completed.")]
    public List<ObjectiveDialogueGroup> hiddenObjectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    public List<Event> eventList = new List<Event>();
    Dictionary<string, List<GameObject>> eventTags = new Dictionary<string, List<GameObject>>();
    ObjectiveSystem objectiveSystem;
    // Tracks total number of ObjDiaGroups created. Used for colors of objective UI
    static int group_num = 0;
    // Start is called before the first frame update
    void Start()
    {
        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();
        foreach (ObjectiveDialogueGroup odg in objectiveDialogueGroups) {
            objectiveSystem.addObjectiveList(odg.objectives, group_num);
        }
        foreach (Event evt in eventList) {
            Debug.Log($"Gamecontroller adding event tag {evt.tag}");
            eventTags.Add(evt.GetTag(), evt.GetGameObjects());
        }
    }

    // Update is called once per frame
    void Update()
    {
        List<ObjectiveDialogueGroup> groupsToRemove = new List<ObjectiveDialogueGroup>();
        // Check if any current ObjDiaGroups are complete, if so: push their dialogue ptrs to respective NPCs and remove them from ObjSys UI
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
        foreach (ObjectiveDialogueGroup odg in hiddenObjectiveDialogueGroups) {
            if (odg.CheckForCompletion()) {
                odg.PushDialoguePointer();
                groupsToRemove.Add(odg);
            }
        }
        foreach (ObjectiveDialogueGroup odg in groupsToRemove) {
            hiddenObjectiveDialogueGroups.Remove(odg);
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

    public void ActivateTag(string tag) {
        if (eventTags.ContainsKey(tag)) {
            Event evt = new Event(tag, eventTags[tag]);
            evt.Activate();
        } else {
            Debug.Log($"A script is attempting to access event tag '{tag}' but it does not exist in the game controller.");
        }
    }

    public void DeactivateTag(string tag) {
        if (eventTags.ContainsKey(tag)) {
            Event evt = new Event(tag, eventTags[tag]);
            evt.Deactivate();
        } else {
            Debug.Log($"A script is attempting to access event tag '{tag}' but it does not exist in the game controller.");
        }
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
        if (pair.npc != null) {
            pair.npc.setDialoguePointer(pair.ptr);
        }
    }
}

// A DialoguePointerMap maps NPCs to dialogue pointers. When an ObjDiaGroup is completed, each NPC-Ptr pair is evaluated and the pointer
// is sent to it's paired NPC to switch that NPCs dialogue tree.
[System.Serializable]
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

[System.Serializable]
public class Event {
    public string tag;
    public List<GameObject> gameObjects;

    public Event(string tag, List<GameObject> gameObjects) {
        this.tag = tag;
        this.gameObjects = gameObjects;
    }

    public string GetTag() {
        return tag;
    }

    public List<GameObject> GetGameObjects() {
        return gameObjects;
    }

    public void Activate() {
        foreach (GameObject obj in gameObjects) {
            obj.SetActive(true);
        }
    }

    public void Deactivate() {
        foreach (GameObject obj in gameObjects) {
            obj.SetActive(false);
        }
    }
}