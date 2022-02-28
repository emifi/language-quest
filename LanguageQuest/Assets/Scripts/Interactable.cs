using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionString = "";
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
        return $"Interaction string not set for {transform.name}";
    }

    public virtual void Highlight(){
        Debug.Log($"Attempting to highlight {transform.name}, but Highlight() method has not been set up for this object.");
    }

    public virtual void Unhighlight(){
        Debug.Log($"Attempting to unhighlight {transform.name}, but Highlight() method has not been set up for this object.");
    }
}
