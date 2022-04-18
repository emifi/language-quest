using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public InventoryObject inventory;
    public NotebookObject notebook;
    public float interactionRange = 2f;
    public float highlightRange = 4f;
    public LayerMask rayMask;

    ObjectiveSystem objectiveSystem;
    Canvas actionUI;
    Canvas notebookUI;
    Canvas dialogueUI;
    Canvas settingsUI;
    GameObject highlightTrigger;
    SphereCollider highlightCollider;
    Transform playerCamera;
    Transform firstPersonPlayer;

    Collider last = null;

    void Start() {
        // UIs
        actionUI = GameObject.Find("ActionUI").GetComponent<Canvas>();
        notebookUI = GameObject.Find("NotebookUI").GetComponent<Canvas>();
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        settingsUI = GameObject.Find("SettingsUI").GetComponent<Canvas>();
        // Other dependencies
        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();
        highlightTrigger = gameObject;
        highlightCollider = highlightTrigger.GetComponent<SphereCollider>();
        highlightTrigger.transform.localScale = new Vector3(highlightRange*2, highlightRange*2, highlightRange*2);
        firstPersonPlayer = transform.parent;
        playerCamera = firstPersonPlayer.Find("Main Camera");
    }


    // Update is called once per frame
    void Update()
    {
        if(!DialogueUI.dialogueComplete()){ //if dialogue occurring, interaction is disallowed
            actionUI.enabled = false;
            if (DialogueUI.textComplete()&&Input.GetKeyDown(KeyCode.E)) {
                DialogueUI.turnPage();
            }else if (!DialogueUI.textComplete()&&Input.GetKeyDown(KeyCode.E)) {
                DialogueUI.skip();
            }
            return;
        }

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
                actionUI.transform.Find("EIndicator/Text").GetComponent<TMP_Text>().text = interactable.GetInteractString();

                // On interact
                if (!notebookUI.enabled&&Input.GetKeyDown(KeyCode.E)) {
                    if (interactable is ItemInteractable) {
                        ItemObject item = (interactable as ItemInteractable).item;
                        if (item.canBeAddedToNotebook) notebook.AddItem(item);
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
                        objectiveSystem.talkToEvent((interactable as NPCInteractable).id);
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

        if (!settingsUI.enabled&&Input.GetKeyDown(KeyCode.N)) {
            NotebookUI.navHome();
            if (notebookUI.isActiveAndEnabled) {
                notebookUI.enabled = false;
                MouseLook.uiClose();
                PlayerMovement.notebookClose();
            } else {
                notebookUI.enabled = true;
                MouseLook.uiOpen();
                PlayerMovement.notebookOpen();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (notebookUI.enabled&&notebookUI.isActiveAndEnabled) {
                notebookUI.enabled = false;
                MouseLook.uiClose();
                PlayerMovement.notebookClose();
            } else if (!settingsUI.isActiveAndEnabled) {
                settingsUI.enabled = true;
                settingsUI.GetComponent<SettingsUI>().onOpen();
                MouseLook.uiOpen();
                PlayerMovement.optionsOpen();
            } else if (settingsUI.isActiveAndEnabled) {
                Time.timeScale = 1;
                settingsUI.enabled = false;
                MouseLook.uiClose();
                PlayerMovement.optionsClose();
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
