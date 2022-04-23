using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    [SerializeField]
    private List<ObjectiveDialogueGroup> objectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    [Header("These groups will not be displayed on the players UI. Useful in non-linear progression for checking if objectives from other groups have been completed.")]
    [SerializeField]
    private List<ObjectiveDialogueGroup> hiddenObjectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    [Header("Events can be fired from dialogue to either active or deactivate gameobjects within the scene")]
    [SerializeField]
    private List<Event> tagList = new List<Event>();
    Dictionary<string, List<GameObject>> eventTags = new Dictionary<string, List<GameObject>>();
    ObjectiveSystem objectiveSystem;
    // Tracks total number of ObjDiaGroups created. Used for colors of objective UI
    static int group_num = 0;
    // Start is called before the first frame update
    void Start()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex-1;

        // Check the DataStructs to see if the current scene has saved ObjectiveDialogueGroups.
        // If no, save the default ObjectiveDialogueGroups that were present in this scene to the DataStructs.
        // If yes, load the saved Objective DialogueGroups from DataStructs into the respective (hidden or otherwise) ObjectiveDialogueGroup container.
        if(DataStructs.objectiveDialogueGroups[currScene]==null){
            List<SavedObjectiveDialogueGroup> saved = new List<SavedObjectiveDialogueGroup>();
            foreach (ObjectiveDialogueGroup odg in objectiveDialogueGroups) {
                if (odg.CheckForCompletion()) continue;
                Debug.Log("Saving ObjDiaGroup...");
                foreach (Objective obj in odg.objectives) {
                    Debug.Log($"- Saving obj {obj.objectiveString}");
                }
                saved.Add(odg.Save());
            }
            DataStructs.objectiveDialogueGroups[currScene] = saved;
        }else{
            List<ObjectiveDialogueGroup> loaded = new List<ObjectiveDialogueGroup>();
            foreach (SavedObjectiveDialogueGroup odg in DataStructs.objectiveDialogueGroups[currScene]) {
                if (odg.CheckForCompletion()) continue;
                Debug.Log("Loading ObjDiaGroup...");
                foreach (Objective obj in odg.objectives) {
                    Debug.Log($"- Loading obj {obj.objectiveString}");
                }
                loaded.Add(odg.Load());
            }
            objectiveDialogueGroups = loaded;
        }
        if(DataStructs.hiddenObjectiveDialogueGroups[currScene]==null){
            List<SavedObjectiveDialogueGroup> saved = new List<SavedObjectiveDialogueGroup>();
            foreach (ObjectiveDialogueGroup odg in hiddenObjectiveDialogueGroups) {
                if (odg.CheckForCompletion()) continue;
                saved.Add(odg.Save());
            }
            DataStructs.hiddenObjectiveDialogueGroups[currScene] = saved;
        }else{
            List<ObjectiveDialogueGroup> loaded = new List<ObjectiveDialogueGroup>();
            foreach (SavedObjectiveDialogueGroup odg in DataStructs.hiddenObjectiveDialogueGroups[currScene]) {
                if (odg.CheckForCompletion()) continue;
                loaded.Add(odg.Load());
            }
            hiddenObjectiveDialogueGroups = loaded;
        }

        // Send the current ObjectiveDialogueGroups to the ObjectiveSystem to be rendered and tracked.
        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();
        foreach (ObjectiveDialogueGroup odg in objectiveDialogueGroups) {
            objectiveSystem.addObjectiveList(odg.objectives, group_num);
        }
        // Send the current hidden ObjectiveDialogueGroups to the ObjectiveSystem to be tracked.
        foreach (ObjectiveDialogueGroup hidodg in hiddenObjectiveDialogueGroups) {
            foreach (Objective hiddenObj in hidodg.objectives) {
                objectiveSystem.addHiddenObjective(hiddenObj);
            }
        }
        // Add all pre-set events to the eventTag container
        foreach (Event evt in tagList) {
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
            DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Remove(odg.Save());
        }
        foreach (ObjectiveDialogueGroup odg in hiddenObjectiveDialogueGroups) {
            if (odg.CheckForCompletion()) {
                odg.PushDialoguePointer();
                groupsToRemove.Add(odg);
            }
        }
        foreach (ObjectiveDialogueGroup odg in groupsToRemove) {
            hiddenObjectiveDialogueGroups.Remove(odg);
            DataStructs.hiddenObjectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Remove(odg.Save());
        }
    }

    /// <summary>
    /// Returns true if there is atleast one active (and visible) ObjectiveDialogueGroup. Otherwise, returns false;
    /// </summary>
    public bool HasActiveGrouping() {
        if (objectiveDialogueGroups.Count > 0) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a new ObjectiveDialogueGroup, saves it to the DataStructs for this scene, and adds it to the ObjectiveSystem to be rendered and tracked.
    /// </summary>
    public void CreateGrouping(List<Objective> objectives, NpcNavMesh npc, int pointer) {
        group_num += 1;
        ObjectiveDialogueGroup group = new ObjectiveDialogueGroup(objectives, npc, pointer);
        SavedObjectiveDialogueGroup saved = group.Save();
        DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Add(saved);
        objectiveSystem.addObjectiveList(objectives, group_num);
        objectiveDialogueGroups.Add(group);
    }
    /// <summary>
    /// Creates a new ObjectiveDialogueGroup, saves it to the DataStructs for this scene, and adds it to the ObjectiveSystem to be rendered and tracked.
    /// </summary>
    public void CreateGrouping(ObjectiveDialogueGroup group) {
        group_num += 1;
        SavedObjectiveDialogueGroup saved = group.Save();
        DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Add(saved);
        objectiveSystem.addObjectiveList(group.objectives, group_num);
        objectiveDialogueGroups.Add(group);
    }

    /// <summary>
    /// Creates a new hidden ObjectiveDialogueGroup, saves it to the DataStructs for this scene, and adds it to the ObjectiveSystem to be tracked.
    /// </summary>
    public void CreateHiddenGrouping(List<Objective> objectives, NpcNavMesh npc, int pointer) {
        ObjectiveDialogueGroup group = new ObjectiveDialogueGroup(objectives, npc, pointer);
        SavedObjectiveDialogueGroup saved = group.Save();
        DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Add(saved);
        hiddenObjectiveDialogueGroups.Add(group);
    }
    /// <summary>
    /// Creates a new hidden ObjectiveDialogueGroup, saves it to the DataStructs for this scene, and adds it to the ObjectiveSystem to be tracked.
    /// </summary>
    public void CreateHiddenGrouping(List<Objective> objectives, List<DialoguePointerMap> ptrMap) {
        ObjectiveDialogueGroup group = new ObjectiveDialogueGroup(objectives, ptrMap);
        SavedObjectiveDialogueGroup saved = group.Save();
        DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Add(saved);
        hiddenObjectiveDialogueGroups.Add(group);
    }
    /// <summary>
    /// Creates a new hidden ObjectiveDialogueGroup, saves it to the DataStructs for this scene, and adds it to the ObjectiveSystem to be tracked.
    /// </summary>
    public void CreateHiddenGrouping(ObjectiveDialogueGroup group) {
        SavedObjectiveDialogueGroup saved = group.Save();
        DataStructs.objectiveDialogueGroups[SceneManager.GetActiveScene().buildIndex-1].Add(saved);
        hiddenObjectiveDialogueGroups.Add(group);
    }

    /// <summary>
    /// Sets the given ObjectiveMisc's ObjectiveStatus to ObjectiveStatus.Complete by firing a miscEvent.
    /// </summary>
    public void ForceComplete(ObjectiveMisc obj) {
        objectiveSystem.miscEvent(obj);
    }

    /// <summary>
    /// (Not to be confused with the Unity Editor built-in tags) Finds the event with the given tag and sets all of its GameObjects to active.
    /// </summary>
    public void ActivateTag(string tag) {
        if (eventTags.ContainsKey(tag)) {
            Event evt = new Event(tag, eventTags[tag]);
            evt.Activate();
        } else {
            Debug.Log($"A script is attempting to access event tag '{tag}' but it does not exist in the game controller.");
        }
    }

    /// <summary>
    /// (Not to be confused with the Unity Editor built-in tags) Finds the event with the given tag and sets all of its GameObjects to inactive.
    /// </summary>
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
    /// <summary>
    /// A GameController class containing a list of Objectives and a DialoguePointerMap. This class allows for dialogue pointers to be sent to NPCs upon the completion of all of its Objectives.
    /// </summary>
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

    /// <summary>
    /// For each DialoguePointerMap pairing, send the dialogue pointer to its respective NPC.
    /// </summary>
    public void PushDialoguePointer() {
        foreach (DialoguePointerMap pair in ptrMap)
        if (pair.npc != null) {
            pair.npc.setDialoguePointer(pair.ptr);
        }
    }

    /// <summary>
    /// Converts all NpcNavMeshs in an ObjectiveDialogueGroup to a string-ified format so that they can be sent the DataStructs and saved between scenes. Returns a SavedObjectiveDialogueGroup.
    /// </summary>
    public SavedObjectiveDialogueGroup Save() {
        List<SavedDialoguePointerMap> newMap = new List<SavedDialoguePointerMap>();
        foreach (DialoguePointerMap pair in ptrMap) {
            if (pair.npc != null) newMap.Add(new SavedDialoguePointerMap(pair.npc.gameObject.name, pair.ptr));
        }
        return new SavedObjectiveDialogueGroup(objectives, newMap, finalObjective);
    }
}
/// <summary>
/// A DialoguePointerMap maps NPCs to dialogue pointers. When an ObjDiaGroup is completed, each NPC-Ptr pair is evaluated and the pointer
/// is used to update its paired NPC's dialogue pointer.
/// </summary>
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