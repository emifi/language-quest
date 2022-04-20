using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    FishingRod rod;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rod = GameObject.Find("Fishing_Rod").GetComponent<FishingRod>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water") && rod.collisionEnabled()) {
            rod.hitWater();
        }
    }
}
