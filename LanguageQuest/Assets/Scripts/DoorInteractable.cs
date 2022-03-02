using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    protected bool isOpen = false;
    protected bool isLocked = false;
    protected float openDegrees = 0f;
    protected float openStep = 0.2f;

    public override void Interact()
    {
        if (isLocked) return;

        if (isOpen && openDegrees <= 0f) {
            Debug.Log("Closing door...");
            isOpen = false;
            openDegrees = 90f;
        } else if (!isOpen && openDegrees <= 0f) {
            Debug.Log("Opening door...");
            isOpen = true;
            openDegrees = 90f;
        }
    }

    public override string GetInteractString()
    {   
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
            transform.RotateAround(transform.position, Vector3.up, -openStep);
            openDegrees -= openStep;
        } else if (!isOpen && openDegrees > 0) {
            transform.RotateAround(transform.position, Vector3.up, openStep);
            openDegrees -= openStep;
        }
    }
}
