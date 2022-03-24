using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float x, y, z;
    public float range;
    public float minGenerationTime = 5f;
    public float maxGenerationTime = 20f;
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
            yield return new WaitForSeconds(Random.Range(minGenerationTime, maxGenerationTime));
            Debug.Log("Getting cloud...");
            GameObject cloud = ObjectPool.SharedInstance.getPooledObject();
            cloud.transform.localScale = new Vector3(Random.Range(1f,2f), Random.Range(0.5f,1.0f), Random.Range(1f,2f));
            if (cloud == null) {
                Debug.Log("Cloud was null");
                yield break;
            }
            cloud.SetActive(true);
            float new_x = Random.Range(-range, range);
            float new_y = Random.Range(-100f, 250f);
            cloud.transform.position = new Vector3(new_x, y + new_y, z);
        }
    }

}
