using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;

public class NPCInteractable : Interactable
{
    public NPCIdentifierObject id;

    new void Start() {

    }
    
    public override void Interact()
    {
        NpcNavMesh npc = gameObject.GetComponent<NpcNavMesh>();
        if (npc == null) npc = gameObject.GetComponentInParent<NpcNavMesh>();
        DialogueUI.initScript(npc);
    }

    public override string GetInteractString()
    {
        if (transform.name == "Root_M") {
            return "to dress Deer";
        }
        return base.GetInteractString();
    }
}
