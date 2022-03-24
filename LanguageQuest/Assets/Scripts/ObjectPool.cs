using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public GameObject cloudObject;
    public int clouds;
    List<GameObject> cloudPool;
    void  Awake() {
        SharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cloudPool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < clouds; i++) {
            tmp = Instantiate(cloudObject);
            tmp.SetActive(false);
            tmp.AddComponent<Cloud>();
            cloudPool.Add(tmp);
        }
    }

    public GameObject getPooledObject() {
        Debug.Log("- Inside gPO");
        for (int i = 0 ; i < clouds ; i++) {
            Debug.Log(i);
            if (cloudPool[i].activeSelf == false) {
                return cloudPool[i];
            }
        }
        Debug.Log("Got nuthin");
        return null;
    }
}
