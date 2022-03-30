using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;

public class NPCInteractable : Interactable
{
    public override void Interact()
    {
        NpcNavMesh npc = gameObject.GetComponent<NpcNavMesh>();
        DialogueUI.setScript(npc);
    }
}
