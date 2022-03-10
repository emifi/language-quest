using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcNavMesh : MonoBehaviour
{
    public Transform[] dest;
    public Transform npc;
    public float minTimeout;
    public float maxTimeout;

    public float colDistance = 0.1f;
    
    private UnityEngine.AI.NavMeshAgent mesh;
    float timeout = 0;
    int destListLength = 0;
    int targetedDest = 0;
    float colTime;
    bool collided = false;
    

    // Start is called before the first frame update
    private void Start()
    {
        destListLength = dest.Length;
        mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if(minTimeout>maxTimeout){
            minTimeout = maxTimeout;
        }
        targetedDest = Random.Range(0,dest.Length);
    }

    // Update is called once per frame
    void Update()
    {
        mesh.destination = dest[targetedDest].position;
        if(collided){
            if(Time.time>=colTime+timeout){
                targetedDest = Random.Range(0,dest.Length);
                collided = false;
            }
        }else if((npc.position - dest[targetedDest].position).sqrMagnitude < .5f){
            colTime = Time.time;
            timeout = Random.Range(minTimeout,maxTimeout);
            collided = true;
        }
    }
}
