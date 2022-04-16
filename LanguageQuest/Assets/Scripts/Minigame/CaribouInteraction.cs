using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaribouInteraction : MonoBehaviour
{
    public InventoryObject inventory;
    public NotebookObject notebook;
    public ObjectiveSystem objectiveSystem;

    public float interactionRange = 8f;
    public float highlightRange = 10f;
    public LayerMask rayMask;
    public Canvas actionUI;
    public Canvas notebookUI;
    public Canvas dialogueUI;
    GameObject highlightTrigger;
    SphereCollider highlightCollider;
    Transform playerCamera;
    Transform firstPersonPlayer;

    Collider last = null;

    void Start() {
        highlightTrigger = gameObject;
        highlightCollider = highlightTrigger.GetComponent<SphereCollider>();
        highlightTrigger.transform.localScale = new Vector3(highlightRange*2, highlightRange*2, highlightRange*2);
        firstPersonPlayer = transform.parent;
        playerCamera = firstPersonPlayer.Find("Main Camera");
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray interactionRay = new Ray(playerCamera.position, playerCamera.forward);
        // If the last raycast was an interactable, unfocus it
        if (last != null && last.GetComponentInParent<Interactable>()) {
            last.GetComponentInParent<Interactable>().Unfocus();
        }
        // If raycast detects an interactable
        if (Physics.Raycast(interactionRay, out hit, interactionRange)) {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable) {
                interactable.Focus();
                actionUI.enabled = true;
                actionUI.transform.Find("EIndicator/Text").GetComponent<Text>().text = interactable.GetInteractString();

                // On interact
                if (!notebookUI.enabled && Input.GetKeyDown(KeyCode.E)) {
                    if (interactable is ItemInteractable) {
                        ItemObject item = (interactable as ItemInteractable).item;
                        if (item.type == ItemType.Pickup) {
                            if (inventory.AddItem(item)) {
                                interactable.Interact();
                                objectiveSystem.pickupEvent(item as PickupObject);
                            }
                        } else if (item.type == ItemType.Trigger) {
                            objectiveSystem.triggerEvent(item as TriggerObject);
                        } else if (item.type == ItemType.Default) {
                            interactable.Interact();
                        }
                    } else if(interactable is NPCInteractable) {
                        dialogueUI.enabled = true;
                        interactable.Interact();
                    } else {
                        interactable.Interact();
                    }
                }
            } else {
                actionUI.enabled = false;
            }
        } else {
            actionUI.enabled = false;
        }
        last = hit.collider;

        if (Input.GetKeyDown(KeyCode.N)) {
            NotebookUI.navHome();
            if (notebookUI.isActiveAndEnabled) {
                notebookUI.enabled = false;
                MouseLook.dictClose();
                PlayerMovement.dictClose();
            } else {
                notebookUI.enabled = true;
                MouseLook.dictOpen();
                PlayerMovement.dictOpen();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (notebookUI.isActiveAndEnabled) {
                notebookUI.enabled = false;
                MouseLook.dictClose();
                PlayerMovement.dictClose();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInParent<Interactable>()) {
            other.GetComponentInParent<Interactable>().Highlight();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponentInParent<Interactable>()) {
            other.GetComponentInParent<Interactable>().Unhighlight();
        }
    }

    private void OnApplicationQuit() {
        inventory.container.Clear();
        notebook.container.Clear();
    }
}