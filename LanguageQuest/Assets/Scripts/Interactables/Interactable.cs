using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

For an interactable to have an outline shader, it must have a Rigid body (and I think it needs to have "Is Kinematic" checked)

*/

public abstract class Interactable : MonoBehaviour
{
    protected Outline outline; 
    public void Start() {
        transform.gameObject.AddComponent<Outline>();
        outline = transform.GetComponent<Outline>();
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 5f;
        outline.enabled = false;
    }

    public virtual void Interact(){
        Debug.Log($"Attempting to interact with {transform.name}, but Interact() method has not been set for this object.");
    }

    public virtual string GetInteractString() {
        return $"to interact with {transform.name}";
    }

    // Default glows/unglows for interactable objects. See PlayerInteraction in player
    public virtual void Highlight(){
        outline.enabled = true;
    }

    public virtual void Unhighlight(){
        outline.enabled = false;
    }

    public virtual void Focus(){
        outline.OutlineColor = Color.blue;
        outline.OutlineWidth = 7f;
    }

    public virtual void Unfocus() {
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 5f;
    }
}
