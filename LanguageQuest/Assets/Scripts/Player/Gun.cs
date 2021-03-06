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
    public TrailRenderer bulletTrail;
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
            muzzleFlash.SetActive(true);
            isFiring = true;
            RaycastHit hit;
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            // If raycast detects an interactable
            if (Physics.Raycast(ray, out hit, range)) {
                TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
                StartCoroutine(createTrail(trail, hit.point));
                // Layer 10 is the wildlife layer (for the deer)
                if (hit.collider.gameObject.layer == 10) {
                    GameObject deer = hit.collider.gameObject;
                    while (!deer.name.StartsWith("Deer")) {
                        deer = deer.transform.parent.gameObject;
                    }
                    Animator animator = deer.GetComponent<Animator>();
                    animator.SetBool("isDead", true);
                    animator.SetBool("isWalking", false);
                    animator.Play("Base Layer.Death", 0);
                    NPCInteractable interactable = deer.AddComponent<NPCInteractable>();
                    interactable.interactOverride = "to dress Caribou";
                    NpcNavMesh navMeshScript = deer.GetComponent<NpcNavMesh>();
                    navMeshScript.walkType = NpcNavMesh.NpcType.Stationary;
                    UnityEngine.AI.NavMeshAgent agent = deer.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    agent.isStopped = true;
                }
                else {
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

    IEnumerator createTrail(TrailRenderer trail, Vector3 target) {
        float bulletSpeed = 125.0f;
        float distance = Vector3.Distance(trail.transform.position, target);
        float time = 0.0f;
        float endTime = distance/bulletSpeed;
        Vector3 startPosition = trail.transform.position;
        //trail.time = endTime;
        while (time < endTime) {
            trail.transform.position = Vector3.Lerp(startPosition, target, time/endTime);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(trail.gameObject);
    }

    GameObject findRoot(string name, GameObject current) {
        GameObject next = null;
        while (current.name != name) {
            next = current.transform.parent.gameObject;
        }
        return next;
    }
}
