using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float x, y, z;
    public float range;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generator());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Generator() {
        while(true) {
            Debug.Log("New cloud...");
            yield return new WaitForSeconds(5);
            Debug.Log("Getting cloud...");
            GameObject cloud = ObjectPool.SharedInstance.getPooledObject();
            cloud.transform.localScale = new Vector3(Random.Range(1f,3f), Random.Range(-1f,1f), Random.Range(-1f,5f));
            if (cloud == null) {
                Debug.Log("Cloud was null");
                yield break;
            }
            cloud.SetActive(true);
            float new_x = Random.Range(-range, range);
            float new_y = Random.Range(-100f, 100f);
            cloud.transform.position = new Vector3(new_x, y + new_y, z);
        }
    }

}
