using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcNavMesh : MonoBehaviour
{
    public Transform dest;
    public Transform npc;

    public float colDistance = 0.1f;
    
    private UnityEngine.AI.NavMeshAgent mesh;
    int minTimeout = 5;
    int maxTimeout = 15;
    

    Vector3 positionChange = new Vector3(0, 0, 1);
    // Start is called before the first frame update
    private void Start()
    {
        mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        mesh.destination = dest.position;
        if((npc.position - dest.position).sqrMagnitude < .5f){
            dest.position+=positionChange;
        }
    }
}
