using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.SceneManagement;

public class NpcNavMesh : MonoBehaviour
{
    public enum NpcType {Roam,Proximity,Stationary,Reactive};

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

    GameObject humanModel;

    NPCPointers np;
    

    // Start is called before the first frame update
    private void Start()
    {
        mesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log(mesh);
        if(destinationGroups!=null&&destinationGroups.Count>0&&destinationGroups[destinationsPtr]!=null){
            
            destListLength = destinationGroups[destinationsPtr].dests.Count;
            if(minTimeout>maxTimeout){
                minTimeout = maxTimeout;
            }
            if(walkType==NpcType.Roam || walkType==NpcType.Reactive){
                targetedDest = Random.Range(0,destListLength);
            }
        }

        player = GameObject.Find("First Person Player").GetComponent<Transform>();
        animator = gameObject.GetComponentInChildren<Animator>();

        Dictionary<string,NPCPointers> npcDict = DataStructs.NPCS[SceneManager.GetActiveScene().buildIndex-1];
        Debug.Log("TEST");
        if(npcDict==null){
            DataStructs.NPCS[SceneManager.GetActiveScene().buildIndex-1] = new Dictionary<string,NPCPointers>();
            npcDict = DataStructs.NPCS[SceneManager.GetActiveScene().buildIndex-1];
            npcDict.Add(this.gameObject.name,new NPCPointers(dialoguePtr,destinationsPtr,targetedDest));
            np = npcDict[this.gameObject.name];
            Debug.Log("A");
        }else{
            if(npcDict.ContainsKey(this.gameObject.name)){
                np = npcDict[this.gameObject.name];
                dialoguePtr = np.dialoguePtr;
                targetedDest = np.targetedDest;
                destinationsPtr = np.destinationsPtr;
                Debug.Log("B");
            }else{
                npcDict.Add(this.gameObject.name,new NPCPointers(dialoguePtr,destinationsPtr,targetedDest));
                np = npcDict[this.gameObject.name];
                Debug.Log("C");
            }
        }

        humanModel = gameObject.GetComponentInChildren<Animator>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.name.StartsWith("Deer") && !gameObject.name.StartsWith("Head")) {
            humanModel.transform.position = new Vector3(transform.position.x, humanModel.transform.position.y, transform.position.z);
        }
        if(animator!=null){
            if (mesh.velocity.magnitude > 0.2f) {
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
            if((walkType==NpcType.Roam || walkType == NpcType.Reactive)&&Time.time>=colTime+timeout){
                targetedDest = Random.Range(0,destinationGroups[destinationsPtr].dests.Count); //assign new random spot to roam
                np.targetedDest = targetedDest;
                collided = false;
            }
            if(walkType==NpcType.Proximity&&Time.time>=colTime+timeout&&(npc.transform.position - player.position).sqrMagnitude < colDistance){ //Nearby player detection
                targetedDest++;
                np.targetedDest = targetedDest;
                if(targetedDest<destinationGroups[destinationsPtr].dests.Count || walkType == NpcType.Reactive){ //If more locations exist, start navigation
                    collided = false;
                }else{ //Otherwise, disallow any more roaming.
                    collided = false;
                    complete = true;
                }
            }
            else if (walkType == NpcType.Reactive && (npc.transform.position - player.position).sqrMagnitude < colDistance) {
                targetedDest = Random.Range(0,destinationGroups[destinationsPtr].dests.Count);
                np.targetedDest = targetedDest;
                collided = false;
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
        FacePlayer();
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
        Debug.Log(dialoguePtr);
        return dialogue[dialoguePtr];
    }

    public void setDialoguePointer(int ptr) {
        if(ptr<0){
            return;
        }
        dialoguePtr = ptr;
        np.dialoguePtr = ptr;
    }

    public void incDialoguePointer() {
        dialoguePtr++;
        np.dialoguePtr++;
    }

    public void setDestinationPointer(int ptr){
        destinationsPtr = ptr;
        np.destinationsPtr = ptr;
    }

    public void FacePlayer() {
        if (true) // gradual change
            StartCoroutine(TurnToPlayer());
        else // instant change
            gameObject.transform.eulerAngles = new Vector3(
                    gameObject.transform.eulerAngles.x,
                    player.eulerAngles.y + 180,
                    gameObject.transform.eulerAngles.z);

    }
    IEnumerator TurnToPlayer() {
        float playerRotationY = player.eulerAngles.y % 360;
        float npcRotationY = gameObject.transform.eulerAngles.y % 360;
        float npcRotationYGoal = (playerRotationY + 180) % 360; // facing player

        if (npcRotationYGoal > npcRotationY) {
            while(npcRotationYGoal > gameObject.transform.eulerAngles.y) {
                gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        gameObject.transform.eulerAngles.y + 0.1f,
                        gameObject.transform.eulerAngles.z);
                yield return null;
            }
        }
        else {
            while(npcRotationYGoal < gameObject.transform.eulerAngles.y) {
                gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        gameObject.transform.eulerAngles.y - 0.1f,
                        gameObject.transform.eulerAngles.z);
                yield return null;
            }
        }
    }
}

[System.Serializable]
public class DestinationArray{
    public List<Transform> dests = new List<Transform>();
}

public class NPCPointers{
    public int dialoguePtr;
    public int targetedDest;
    public int destinationsPtr;
    public NPCPointers(int diap, int td, int desp){
        dialoguePtr = diap;
        targetedDest = td;
        destinationsPtr = desp;
    }
}
