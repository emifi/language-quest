using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float speed = 20f;
    public float range = 5f;
    public float maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        speed = speed + Random.Range(-range, range);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z <= maxDistance) {
            transform.gameObject.SetActive(false);
        } else {
            Vector3 curr = transform.position;
            transform.position = new Vector3(curr.x, curr.y, curr.z - (speed * Time.deltaTime));
        }

    }

    public void simulate(float time) {
        Vector3 curr = transform.position;
        transform.position = new Vector3(curr.x, curr.y, curr.z - (speed * time));
    }
}
