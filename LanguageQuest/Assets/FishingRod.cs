using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    float delayBetweenCasts = 2.0f;
    bool readyToCast = true;
    bool reeling = false;

    bool casting = false;
    bool cast = false;
    bool set = false;
    bool nibble = false;
    Transform camera;
    GameObject bobber;
    Rigidbody bobberRB;
    Transform rod;
    Transform rodTip;
    LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").transform;
        rod = gameObject.transform;  
        rodTip = rod.Find("Rod_Tip");
        bobber = GameObject.Find("Bobber");
        bobberRB = bobber.GetComponent<Rigidbody>();
        bobberRB.useGravity = false;
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
            Debug.Log($"Clicked: readyToCast = {readyToCast} reeling = {reeling}");
            if (readyToCast && !cast && !reeling) {
                readyToCast = false;
                StartCoroutine(Cast(1.0f, 0.4f));
            } else if (!readyToCast && cast && !reeling) {
                reeling = true;
                StartCoroutine(Reel(0.4f, 1.0f));
            }
        }
        line.SetPosition(0, rodTip.transform.position);
        line.SetPosition(1, bobber.transform.position);
    }

    public void hitWater() {
        set = true;
        Debug.Log("Bobber set.");
        bobberRB.useGravity = false;
        bobberRB.velocity = Vector3.zero;
    }

    IEnumerator Cast(float pullBackTime, float pushForwardTime) {
        Quaternion resting = rod.localRotation;
        float t = 0.0f;
        Debug.Log("Casting..");
        while (t < pullBackTime) {
            rod.localRotation = Quaternion.Slerp(resting, Quaternion.Euler(-120.0f, 0.0f, -90.0f), t/pullBackTime);
            bobber.transform.position = rodTip.position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < pushForwardTime) {
            rod.localRotation = Quaternion.Slerp(Quaternion.Euler(-120.0f, 0.0f, -90.0f), resting, t/pushForwardTime);
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
        Quaternion resting = rod.localRotation;
        float t = 0.0f;
        bobberRB.useGravity = false;
        bobberRB.velocity = Vector3.zero;
        Vector3 bobberOriginal = bobber.transform.position;
        Debug.Log("Reeling..");
        while (t < pullBackTime) {
            rod.localRotation = Quaternion.Slerp(resting, Quaternion.Euler(-100.0f, 0.0f, -90.0f), t/pullBackTime);
            bobber.transform.position = Vector3.Lerp(bobberOriginal, rodTip.position, t/pullBackTime);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < pushForwardTime) {
            rod.localRotation = Quaternion.Slerp(Quaternion.Euler(-100.0f, 0.0f, -90.0f), resting, t/pushForwardTime);
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
}
