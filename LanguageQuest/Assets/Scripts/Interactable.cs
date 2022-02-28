using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Interact(){
        Debug.Log($"Attempting to interact with {transform.name}, but Interact() method has not been set for this object.");
    }

    public virtual void Highlight(){
        Debug.Log($"Attempting to highlight {transform.name}, but Highlight() method has not been set up for this object.");
    }
}
