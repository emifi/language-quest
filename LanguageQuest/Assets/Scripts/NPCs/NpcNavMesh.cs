using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;

public class NpcNavMesh : MonoBehaviour
{
    public enum NpcType {Roam,Proximity,Stationary};

    public List<DestinationArray> destinationGroups;
    public GameObject npc;
    public Transform player;
    public float minTimeout;
    public float maxTimeout;
    public NpcType walkType;
    public DialogueContainer[] dialogue;

    public float colDistance = 0.5f;

    public static int numDests = 0;
    
    private UnityEngine.AI.NavMeshAgent mesh;
    private Animator animator;
    public int dialoguePtr = 0; //points to which dialogue in the dialogue container is active
    public int destinationsPtr = 0;
    float timeout = 0;
    int destListLength = 0;
    int targetedDest = 0;
    float colTime;
    bool collided = false;
    bool complete = false;
    

    // Start is called before the first frame update
    private void Start()
    {
        mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log(mesh);
        if(destinationGroups!=null&&destinationGroups.Count>0&&destinationGroups[destinationsPtr]!=null){
            Debug.Log("Made it here start 1");
            
            destListLength = destinationGroups[destinationsPtr].dests.Count;
            if(minTimeout>maxTimeout){
                minTimeout = maxTimeout;
            }
            if(walkType==NpcType.Roam){
                targetedDest = Random.Range(0,destListLength);
            }else{
                MeshRenderer renderer = npc.GetComponent<MeshRenderer>();
                renderer.material.color = Color.blue;
            }
        }
        player = GameObject.Find("First Person Player").GetComponent<Transform>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator!=null){
            if (mesh.velocity.magnitude > 0.1f) {
                animator.SetBool("isWalking", true);
            } else {
                animator.SetBool("isWalking", false);
            }
        }

        if(walkType==NpcType.Stationary){
            return;
        }
        if(!complete){ //Hidden away from complete proximity NPC
            mesh.destination = destinationGroups[destinationsPtr].dests[targetedDest].position;
        }
        if(collided){ //If collision, react to timeout or nearby player
            if(walkType==NpcType.Roam&&Time.time>=colTime+timeout){
                targetedDest = Random.Range(0,destinationGroups[destinationsPtr].dests.Count); //assign new random spot to roam
                collided = false;
            }
            if(walkType==NpcType.Proximity&&Time.time>=colTime+timeout&&(npc.transform.position - player.position).sqrMagnitude < colDistance){ //Nearby player detection
                targetedDest++;
                if(targetedDest<destinationGroups[destinationsPtr].dests.Count){ //If more locations exist, start navigation
                    collided = false;
                }else{ //Otherwise, disallow any more roaming.
                    MeshRenderer renderer = npc.GetComponent<MeshRenderer>();
                    renderer.material.color = Color.red;
                    collided = false;
                    complete = true;
                }
            }
        }else if((!complete&&(npc.transform.position - destinationGroups[destinationsPtr].dests[targetedDest].position).sqrMagnitude < colDistance)){ //Upon collision, take time and determine timeout
            colTime = Time.time;
            timeout = Random.Range(minTimeout,maxTimeout);
            collided = true;
        }
    }

    public int getDestinations(){
        return destinationGroups[destinationsPtr].dests.Count;
    }

    public void stopMovement(){
        mesh.velocity = Vector3.zero;
    }

    public NpcType getType(){
        return walkType;
    }

    public void setType(NpcType t){
        walkType =t;
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

    public void setDestinationPointer(int ptr){
        destinationsPtr = ptr;
    }
}

[System.Serializable]
public class DestinationArray{
    public List<Transform> dests = new List<Transform>();
}