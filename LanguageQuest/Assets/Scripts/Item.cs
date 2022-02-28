using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public override void Interact()
    {
        Material material = transform.GetComponent<Renderer>().material;
        material.color = new Color(
            Random.Range(0f,1f),
            Random.Range(0f,1f),
            Random.Range(0f,1f)
            );
    }

    public override string GetInteractString()
    {
        if (interactionString != "") {
            return interactionString;
        }
        return $"to interact with {transform.name}";
    }

    public override void Highlight()
    {
        outline.enabled = true;
    }

    public override void Unhighlight()
    {
        outline.enabled = false;
    }
}
