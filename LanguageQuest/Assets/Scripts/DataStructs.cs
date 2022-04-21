using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DataStructs
{
    public static List<ObjectiveDialogueGroup>[] objectiveDialogueGroups = new List<ObjectiveDialogueGroup>[SceneManager.sceneCountInBuildSettings];
    public static List<ObjectiveDialogueGroup>[] hiddenObjectiveDialogueGroups = new List<ObjectiveDialogueGroup>[SceneManager.sceneCountInBuildSettings];
    public static InventoryObject inventory = Resources.Load<InventoryObject>("Inventory/Player Inventory");
    public static NotebookObject notebook = Resources.Load<NotebookObject>("Inventory/Player Notebook");
    public static string playerName = "Player";
    public static ChangeTimeOfDay.TimeType[] timeList = new ChangeTimeOfDay.TimeType[SceneManager.sceneCountInBuildSettings];
}
