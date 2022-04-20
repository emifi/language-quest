using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public float minBiteTime = 3.0f;
    public float maxBiteTime = 15.0f;
    float delayBetweenCasts = 2.0f;
    float timeUntilBite;
    bool readyToCast = true;
    bool reeling = false;
    bool cast = false;
    bool set = false;
    bool nibble = false;
    bool bite = false;

    InventoryObject inventory;
    Transform camera;
    GameObject bobber;
    Rigidbody bobberRB;
    SphereCollider bobberCollider;
    Transform rod;
    Transform rodTip;
    LineRenderer line;
    ParticleSystem ripple;
    GameObject fish;
    Quaternion rodResting;
    ObjectiveSystem objectiveSystem;

    // Start is called before the first frame update
    void Start()
    {
        objectiveSystem = GameObject.Find("First Person Player").GetComponent<ObjectiveSystem>();
        inventory = DataStructs.inventory;
        camera = GameObject.Find("Main Camera").transform;
        rod = gameObject.transform;  
        rodTip = rod.Find("Rod_Tip");
        rodResting = rod.localRotation;
        bobber = GameObject.Find("Bobber");
        bobberCollider = bobber.GetComponent<SphereCollider>();
        bobberRB = bobber.GetComponent<Rigidbody>();
        bobberRB.useGravity = false;
        ripple = GameObject.Find("FX_WaterSplatter").GetComponent<ParticleSystem>();
        
        fish = GameObject.Find("Fish");
        fish.SetActive(false);
        line = bobber.GetComponentInChildren<LineRenderer>(); 
        line.SetPosition(0, rodTip.transform.position);
        line.SetPosition(1, bobber.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!set && !cast && readyToCast) {
            bobber.transform.position = rodTip.position;
        }

        // On input mouse left click
        if (Input.GetMouseButton(0) && Time.timeScale != 0) {

            // Player clicked to cast their rod
            if (readyToCast && !cast && !reeling) {
                readyToCast = false;
                StopAllCoroutines();
                StartCoroutine(Cast(1.0f, 0.4f));
            } 

            // Player clicked to reel in their rod
            else if (!readyToCast && cast && !reeling) {
                if (set && bite) {
                    bite = false;
                    fish.SetActive(true);
                    fish.transform.rotation = Quaternion.LookRotation(rodTip.transform.position, Vector3.up);
                    StopAllCoroutines();
                    StartCoroutine(Reel(0.4f, 1.0f));
                } else {
                    reeling = true;
                    StopAllCoroutines();
                    StartCoroutine(Reel(0.4f, 1.0f));
                }
            }
        }

        line.SetPosition(0, rodTip.transform.position);
        line.SetPosition(1, bobber.transform.position);
    }

    public void hitWater() {
        set = true;
        timeUntilBite = Random.Range(minBiteTime, maxBiteTime);
        bobberRB.useGravity = false;
        bobberRB.velocity = Vector3.zero;
        ripple.Play();
        StartCoroutine(Bob(20.0f, 1.0f));
    }

    public bool collisionEnabled() {
        return !set;
    }

    IEnumerator Cast(float pullBackTime, float pushForwardTime) {
        float t = 0.0f;
        while (t < pullBackTime) {
            rod.localRotation = Quaternion.Slerp(rodResting, Quaternion.Euler(-120.0f, 0.0f, -90.0f), t/pullBackTime);
            bobber.transform.position = rodTip.position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < pushForwardTime) {
            rod.localRotation = Quaternion.Slerp(Quaternion.Euler(-120.0f, 0.0f, -90.0f), rodResting, t/pushForwardTime);
            bobber.transform.position = rodTip.position;
            t += Time.deltaTime;
            yield return null;
        }
        bobberRB.useGravity = true;
        bobberRB.AddForce((camera.forward * 700) + (camera.up * 700));
        readyToCast = false;
        reeling = false;
        cast = true;
        set = false;
        yield return null;
    }

    IEnumerator Reel(float pullBackTime, float pushForwardTime) {
        float t = 0.0f;
        bobberRB.useGravity = false;
        bobberRB.velocity = Vector3.zero;
        Vector3 bobberOriginal = bobber.transform.position;
        while (t < pullBackTime) {
            rod.localRotation = Quaternion.Slerp(rodResting, Quaternion.Euler(-100.0f, 0.0f, -90.0f), t/pullBackTime);
            bobber.transform.position = Vector3.Lerp(bobberOriginal, rodTip.position, t/pullBackTime);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        if (fish.activeSelf) {
            fish.SetActive(false);
            Debug.Log("Adding fish to inv");
            PickupObject fishItem = Resources.Load<PickupObject>("Items/Fish");
            inventory.AddItem(fishItem);
            objectiveSystem.pickupEvent(fishItem);
        }
        while (t < pushForwardTime) {
            rod.localRotation = Quaternion.Slerp(Quaternion.Euler(-100.0f, 0.0f, -90.0f), rodResting, t/pushForwardTime);
            bobber.transform.position = rodTip.position;
            t += Time.deltaTime;
            yield return null;
        }
        readyToCast = true;
        reeling = false;
        cast = false;
        set = false;
        yield return null;
    }

    IEnumerator Bob(float smoothness, float frequency) {
        float angle = 0.0f;
        Vector3 root = bobber.transform.position;
        float timeSinceStart = 0.0f;
        while (true) {
            while (angle < 2 * Mathf.PI) {
                if (!bite) {
                    bobber.transform.position = new Vector3(root.x, root.y + Mathf.Sin(angle)/smoothness, root.z);
                    angle += Time.deltaTime * (2 * Mathf.PI * frequency);
                    timeSinceStart += Time.deltaTime;
                }
                yield return null;
            }
            if (timeSinceStart > timeUntilBite) {
                StartCoroutine(Bite(root));
                yield break;
            }
            angle = 0.0f;
        }
    }

    IEnumerator Nibble() {
        yield return null;
    }

    IEnumerator Bite(Vector3 root) {
        float t = 0.0f;
        bite = true;
        Vector3 submerged = root + (new Vector3(0.0f, -0.8f, 0.0f));
        while (t < 0.6f) {
            bobber.transform.position = Vector3.Lerp(root, submerged, t/0.3f);
            t += Time.deltaTime;
            yield return null;
        }
        bite = false;
        timeUntilBite = Random.Range(minBiteTime, maxBiteTime);
        t = 0.0f;
        while (t < 0.3f) {
            bobber.transform.position = Vector3.Lerp(submerged, root, t/0.3f);
            t += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Bob(20.0f, 1.0f));
        yield return null;
    }
}
