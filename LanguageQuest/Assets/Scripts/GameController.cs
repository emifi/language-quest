using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<ObjectiveDialogueGroup> objectiveDialogueGroups = new List<ObjectiveDialogueGroup>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ObjectiveDialogueGroup odg in objectiveDialogueGroups) {
            if (odg.CheckForCompletion()) odg.PushDialoguePointer();
        }
    }
}

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