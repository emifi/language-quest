using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    protected bool isOpen = false;
    protected bool isLocked = false;
    protected float openDegrees = 0f;
    protected float openStep = 120.0f;

    public override void Interact()
    {
        if (isLocked) return;

        if (isOpen) {
            Debug.Log("Closing door...");
            isOpen = false;
            openDegrees = 90f - openDegrees;
        } else if (!isOpen) {
            Debug.Log("Opening door...");
            isOpen = true;
            openDegrees = 90f - openDegrees;
        }
    }

    public override string GetInteractString()
    {   
        if (isLocked) {
            return $"locked {transform.name}";
        }
        if (isOpen) {
            return $"to close {transform.name}";
        }
        return $"to open {transform.name}";
    }

    public override void Highlight()
    {
        outline.enabled = true;
    }

    public override void Unhighlight()
    {
        outline.enabled = false;
    }

    public void Update() {
        if (isOpen && openDegrees > 0) {
            float min = Mathf.Min(openStep*Time.deltaTime, openDegrees);
            transform.RotateAround(transform.position, Vector3.up, -min);
            openDegrees -= min;
        } else if (!isOpen && openDegrees > 0) {
            float min = Mathf.Min(openStep*Time.deltaTime, openDegrees);
            transform.RotateAround(transform.position, Vector3.up, min);
            openDegrees -=min;
        }
    }
}
