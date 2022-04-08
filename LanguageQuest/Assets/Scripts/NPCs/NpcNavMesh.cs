using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;

public class NpcNavMesh : MonoBehaviour
{
    public enum NpcType {Roam,Proximity,Stationary};

    public Transform[] dest;
    public GameObject npc;
    public Transform player;
    public float minTimeout;
    public float maxTimeout;
    public NpcType walkType;
    public DialogueContainer[] dialogue;

    public float colDistance = 0.5f;

    public static int numDests = 0;
    
    private UnityEngine.AI.NavMeshAgent mesh;
    public int dialoguePtr = 0; //points to which dialogue in the dialogue container is active
    float timeout = 0;
    int destListLength = 0;
    int targetedDest = 0;
    float colTime;
    bool collided = false;
    bool complete = false;
    

    // Start is called before the first frame update
    private void Start()
    {
        if(walkType!=NpcType.Stationary){
        destListLength = dest.Length;
        mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if(minTimeout>maxTimeout){
            minTimeout = maxTimeout;
        }
        if(walkType==NpcType.Roam){
            targetedDest = Random.Range(0,dest.Length);
        }else{
            MeshRenderer renderer = npc.GetComponent<MeshRenderer>();
            renderer.material.color = Color.blue;
        }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(walkType==NpcType.Stationary){
            return;
        }
        if(!complete){ //Hidden away from complete proximity NPC
            mesh.destination = dest[targetedDest].position;
        }
        
        if(collided){ //If collision, react to timeout or nearby player
            if(walkType==NpcType.Roam&&Time.time>=colTime+timeout){
                targetedDest = Random.Range(0,dest.Length); //assign new random spot to roam
                collided = false;
            }
            if(walkType==NpcType.Proximity&&Time.time>=colTime+timeout&&(npc.transform.position - player.position).sqrMagnitude < colDistance){ //Nearby player detection
                targetedDest++;
                if(targetedDest<dest.Length){ //If more locations exist, start navigation
                    collided = false;
                }else{ //Otherwise, disallow any more roaming.
                    MeshRenderer renderer = npc.GetComponent<MeshRenderer>();
                    renderer.material.color = Color.red;
                    collided = false;
                    complete = true;
                }
            }
        }else if((!complete&&(npc.transform.position - dest[targetedDest].position).sqrMagnitude < colDistance)){ //Upon collision, take time and determine timeout
            colTime = Time.time;
            timeout = Random.Range(minTimeout,maxTimeout);
            collided = true;
        }
    }

    public int getDestinations(){
        return dest.Length;
    }

    public void stopMovement(){
        mesh.velocity = Vector3.zero;
    }

    public NpcType getType(){
        return walkType;
    }

    public void turnDialogue(){
        dialoguePtr++;
    }

    public DialogueContainer getCurrDialogue(){
        return dialogue[dialoguePtr];
    }

    public void setDialoguePointer(int ptr) {
        dialoguePtr = ptr;
    }
}
