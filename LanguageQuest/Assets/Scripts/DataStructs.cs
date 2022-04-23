using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DataStructs
{
    public static List<SavedObjectiveDialogueGroup>[] objectiveDialogueGroups = new List<SavedObjectiveDialogueGroup>[SceneManager.sceneCountInBuildSettings];
    public static List<SavedObjectiveDialogueGroup>[] hiddenObjectiveDialogueGroups = new List<SavedObjectiveDialogueGroup>[SceneManager.sceneCountInBuildSettings];
    public static Dictionary<string,NPCPointers>[] NPCS = new Dictionary<string,NPCPointers>[SceneManager.sceneCountInBuildSettings];
    public static InventoryObject inventory = Resources.Load<InventoryObject>("Inventory/Player Inventory");
    public static NotebookObject notebook = Resources.Load<NotebookObject>("Inventory/Player Notebook");
    public static string playerName = "Player";
    public static Vector3 playerPos;
    public static Quaternion playerRot;
    public static ChangeTimeOfDay.TimeType[] timeList = new ChangeTimeOfDay.TimeType[SceneManager.sceneCountInBuildSettings];
}

public class SavedObjectiveDialogueGroup {
    public List<Objective> objectives = new List<Objective>();
    public Objective finalObjective;
    public List<SavedDialoguePointerMap> ptrMap = new List<SavedDialoguePointerMap>();

    public SavedObjectiveDialogueGroup(List<Objective> objectives, List<SavedDialoguePointerMap> ptrMap, Objective finalObjective = null) {
        this.objectives = objectives;
        this.ptrMap = ptrMap;
        this.finalObjective = finalObjective;
    }

    public bool CheckForCompletion() {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Incomplete) {
                return false;
            }
        }
        return true;
    }

    public ObjectiveDialogueGroup Load() {
        List<DialoguePointerMap> newMap = new List<DialoguePointerMap>();
        foreach (SavedDialoguePointerMap pair in ptrMap) {
            newMap.Add(new DialoguePointerMap(GameObject.Find(pair.npc).GetComponent<NpcNavMesh>(), pair.ptr));
        }
        return new ObjectiveDialogueGroup(objectives, newMap, finalObjective);
    }
}

public class SavedDialoguePointerMap {
    public string npc;
    public int ptr;

    public SavedDialoguePointerMap(string npc, int ptr) {
        this.npc = npc;
        this.ptr = ptr;
    }
}
