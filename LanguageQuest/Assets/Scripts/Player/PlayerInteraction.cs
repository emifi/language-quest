using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public InventoryObject inventory;

    public float interactionRange = 2f;
    public float highlightRange = 4f;
    public LayerMask rayMask;
    public Canvas tempUI;
    GameObject highlightTrigger;
    SphereCollider highlightCollider;
    Transform playerCamera;
    Transform firstPersonPlayer;

    Collider last = null;

    void Start() {
        highlightTrigger = gameObject;
        highlightCollider = highlightTrigger.AddComponent<SphereCollider>();
        highlightTrigger.transform.localScale = new Vector3(highlightRange*2, highlightRange*2, highlightRange*2);
        firstPersonPlayer = transform.parent;
        playerCamera = firstPersonPlayer.Find("Main Camera");
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
                tempUI.enabled = true;
                tempUI.transform.Find("EIndicator/Text").GetComponent<Text>().text = interactable.GetInteractString();

                // On interact
                if (Input.GetKeyDown(KeyCode.E)) {
                    if (interactable is ItemInteractable) {
                        ItemObject item = (interactable as ItemInteractable).item;
                        if (item.type == ItemType.Pickup) {
                            if (inventory.AddItem(item)) {
                                interactable.Interact();
                            }
                        } else if (item.type == ItemType.Entry) {

                        } else if (item.type == ItemType.Trigger) {

                        } else if (item.type == ItemType.Default) {
                            interactable.Interact();
                        }
                    } else {
                        interactable.Interact();
                    }
                }
            } else {
                tempUI.enabled = false;
            }
        } else {
            tempUI.enabled = false;
        }
        last = hit.collider;
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
    }
}
