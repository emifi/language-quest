using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> cloudObjects;
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
        int rotation = 0;
        for (int i = 0; i < clouds; i++) {
            if (rotation >= cloudObjects.Count) rotation = 0;
            tmp = Instantiate(cloudObjects[rotation]);
            tmp.SetActive(false);
            tmp.AddComponent<Cloud>();
            cloudPool.Add(tmp);
            rotation++;
        }
    }

    public GameObject getPooledObject() {
        for (int i = 0 ; i < clouds ; i++) {
            if (cloudPool[i].activeSelf == false) {
                return cloudPool[i];
            }
        }
        return null;
    }
}
