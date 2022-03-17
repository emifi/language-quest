using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public string[] tempScript;
    public override void Interact()
    {
        tempScript = new string[2]{$"Hello, my name is {gameObject.name}!",$"I have {gameObject.GetComponent<NpcNavMesh>().getDestinations()} destinations!"};
        DialogueUI.setScript(tempScript);
    }
}
