using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataStructs
{
    public static InventoryObject inventory = Resources.Load<InventoryObject>("Inventory/Player Inventory");
    public static NotebookObject notebook = Resources.Load<NotebookObject>("Inventory/Player Notebook");
    public static string playerName = "";
}
