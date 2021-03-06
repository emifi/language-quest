using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public ItemObject item;
    public string overrideInteractionString = "";
    public override void Interact()
    {
        if (item.type == ItemType.Default) {
            Material material = transform.GetComponentInChildren<Renderer>().material;
            material.color = new Color(
                Random.Range(0f,1f),
                Random.Range(0f,1f),
                Random.Range(0f,1f)
                );
        }
        else if (item.type == ItemType.Pickup) {
            if (item == Resources.Load<ItemObject>("Items/Caribou")) {
                DialogueUI.initScript(GameObject.Find("Deer").GetComponent<NpcNavMesh>());
                Destroy(gameObject.transform.parent.parent.parent.gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    public override string GetInteractString()
    {
        if (overrideInteractionString != "") {
            return overrideInteractionString;
        }
        return item.interactionString + " " + item.nativeName;
    }
}
