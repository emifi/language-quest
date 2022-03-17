using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public string[] tempScript;
    public override void Interact()
    {
        NpcNavMesh npc = gameObject.GetComponent<NpcNavMesh>();
        tempScript = new string[2]{$"Hello, my name is {gameObject.name}!",$"I have {npc.getDestinations()} destinations!"};
        DialogueUI.setScript(tempScript,npc);
    }
}
