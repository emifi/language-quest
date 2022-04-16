using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Transform playerCamera;
    GameObject muzzleFlash;
    bool isFiring = false;
    float fireTime = 0.0f;
    public float range;
    float fireDelay = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = GameObject.Find("FX_GunFlash");
        muzzleFlash.SetActive(false);
        playerCamera = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFiring && Input.GetMouseButtonDown(0)) {
            Debug.Log("BANG");
            muzzleFlash.SetActive(true);
            isFiring = true;
            RaycastHit hit;
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            // If raycast detects an interactable
            if (Physics.Raycast(ray, out hit, range)) {
                if (hit.collider.gameObject.layer == 10) {
                    Animator animator = hit.collider.GetComponentInParent<Animator>();
                    animator.SetBool("isDead", true);
                    ItemInteractable interactable = hit.collider.gameObject.AddComponent<ItemInteractable>();
                    interactable.item = Resources.Load<ItemObject>("Items/Caribou");
                    Debug.Log("Hit!");
                }
                else {
                    Debug.Log("Miss!");
                }
            }
        } else if (isFiring) {
            fireTime += Time.deltaTime;
            if (fireTime > fireDelay) {
                fireTime = 0.0f;
                isFiring = false;
                muzzleFlash.SetActive(false);
            }
        }
    }

    IEnumerator flash() {
        yield return 0;
    }

    GameObject findRoot(string name, GameObject current) {
        GameObject next = null;
        while (current.name != name) {
            next = current.transform.parent.gameObject;
        }
        return next;
    }
}
