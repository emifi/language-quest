using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float x, y, z;
    public float range;
    public float minGenerationTime = 5f;
    public float maxGenerationTime = 20f;
    public float maxDistance = -4000.0f;
    public bool prerunSimulation = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generator());
    }

    // Update is called once per frame
    void Update()
    {
        if (prerunSimulation) {
            preSimulate(200.0f);
        }
    }

    private IEnumerator Generator() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(minGenerationTime, maxGenerationTime));
            GameObject cloud = ObjectPool.SharedInstance.getPooledObject();
            cloud.transform.localScale = new Vector3(Random.Range(1f,2f), Random.Range(0.5f,1.0f), Random.Range(1f,2f));
            if (cloud == null) {
                yield break;
            }
            cloud.SetActive(true);
            float new_x = Random.Range(-range, range);
            float new_y = Random.Range(-100f, 250f);
            cloud.transform.position = new Vector3(new_x, y + new_y, z);
            cloud.GetComponent<Cloud>().maxDistance = maxDistance;
        }
    }

    public void preSimulate(float time) {
        float remainingTime = time;
        while(remainingTime > 0.0f) {
            GameObject cloud = ObjectPool.SharedInstance.getPooledObject();
            cloud.transform.localScale = new Vector3(Random.Range(1f,2f), Random.Range(0.5f,1.0f), Random.Range(1f,2f));
            cloud.SetActive(true);
            float new_x = Random.Range(-range, range);
            float new_y = Random.Range(-100f, 250f);
            cloud.transform.position = new Vector3(new_x, y + new_y, z);
            cloud.GetComponent<Cloud>().simulate(remainingTime);
            remainingTime -= Random.Range(minGenerationTime, maxGenerationTime);
            cloud.GetComponent<Cloud>().maxDistance = maxDistance;
        }
        prerunSimulation = false;
    }

}
