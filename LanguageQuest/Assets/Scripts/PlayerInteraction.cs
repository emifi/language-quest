using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f;
    public float highlightRange = 6f;
    public LayerMask rayMask;
    public Canvas tempUI;
    GameObject highlightTrigger;
    SphereCollider highlightCollider;
    Transform playerCamera;
    Transform firstPersonPlayer;

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
        if (Physics.Raycast(interactionRay, out hit, interactionRange, rayMask)) {
            //Debug.Log("Detect object.");
            if (hit.collider.GetComponent<Interactable>()) {
                //Debug.Log("Detected Interactable.");
                tempUI.enabled = true;
                tempUI.transform.Find("EIndicator/Text").GetComponent<Text>().text = hit.collider.GetComponent<Interactable>().GetInteractString();
                if (Input.GetKeyDown(KeyCode.E)) {
                    hit.collider.GetComponent<Interactable>().Interact();
                }
            }
        } else {
            tempUI.enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log($"Entering: {other.name}");
        if (other.GetComponent<Interactable>()) {
            other.GetComponent<Interactable>().Highlight();
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log($"Exiting: {other.name}");
        if (other.GetComponent<Interactable>()) {
            other.GetComponent<Interactable>().Unhighlight();
        }
    }
}
