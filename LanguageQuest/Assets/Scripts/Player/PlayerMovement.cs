using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool canMove = true;
    public static bool canInput = true;
    public bool showGroundDetection = true;
    public float playerSpeed = 1.5f;
    public float jumpHeight = 5.0f;
    public float gravity = 9.8f;
    public float groundDistance = 0.4f;
    public float headDistance = 0.4f;
    public LayerMask terrainMask;
    GameObject player;
    CharacterController playerController;
    Transform groundCheck;
    Transform headCheck;
    Transform cam;
    Transform spawnpoint;
    Canvas dialogueUI;
    Transform root;
    Vector3 velocity;
    bool isMoving;
    bool isGrounded;
    bool hitHead;
    bool isCrouch;
    bool forcedDown;

    Vector3 positionChange = new Vector3(0, 0.5f, 0);

    GameObject groundDectector;
    GameObject playerModel;

    // Start is called before the first frame update
    void Start()
    {
        // Find player model dependencies
        player = GameObject.Find("First Person Player");
        playerModel = GameObject.Find("Player");
        playerController = player.GetComponent<CharacterController>();
        root = GameObject.Find("CameraRoot").transform;
        cam = GameObject.Find("Main Camera").transform;
        spawnpoint = GameObject.Find("Spawnpoint").GetComponent<Transform>();
        groundCheck = GameObject.Find("GroundCheck").transform;
        headCheck = GameObject.Find("HeadCheck").transform;

        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        if (showGroundDetection) {
            groundDectector = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            groundDectector.name = "Debug Ground Detector";
            groundDectector.transform.parent = transform;
        }

        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();

        player.transform.position = spawnpoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, terrainMask);
        hitHead = Physics.CheckSphere(headCheck.position, headDistance, terrainMask);

        if (showGroundDetection) {
            groundDectector.SetActive(true);
            groundDectector.transform.position = groundCheck.position;
            groundDectector.transform.localScale = new Vector3(groundDistance*2, groundDistance*2, groundDistance*2);
            MeshRenderer renderer = groundDectector.GetComponent<MeshRenderer>();
            SphereCollider collider = groundDectector.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            if (isGrounded) {
                renderer.material.color = Color.green;
            } else {
                renderer.material.color = Color.red;
            }
        } 

        // If grounded and has a negative velocity, set velocity to some small negative amount to force character down. Else apply gravity
        if (isGrounded && velocity.y < 0.0f) {
            velocity.y = -2.0f;
        } else {
            velocity.y -= gravity * Time.deltaTime;
        }

        if (hitHead && velocity.y > 0) {
            Debug.Log("hit head");
            velocity.y = -1.0f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && canInput) {
            Jump();
        }

        float x = 0f;
        float z = 0f;

        NpcNavMesh.NpcType npcType = NpcNavMesh.NpcType.Stationary;
        bool chatBlock = true;

        if(dialogueUI.enabled){
            npcType = DialogueUI.getNpcType();
            chatBlock = !(dialogueUI.enabled&&npcType!=NpcNavMesh.NpcType.Proximity);
        }

        if (Input.GetKey(KeyCode.A)&&chatBlock && canInput) {
            x += -1.0f;
        }

        if(Input.GetKeyDown(KeyCode.A)&&!chatBlock && canInput){
            DialogueUI.scrollX(-1);
        }

        if (Input.GetKey(KeyCode.D)&&chatBlock && canInput) {
            x += 1.0f;
        }

        if(Input.GetKeyDown(KeyCode.D)&&!chatBlock && canInput){
            DialogueUI.scrollX(1);
        }

        if (Input.GetKey(KeyCode.S)&&chatBlock && canInput) {
            z += -1.0f;
        }

        if(Input.GetKeyDown(KeyCode.S)&&!chatBlock && canInput){
            DialogueUI.scrollY(1);
        }

        if (Input.GetKey(KeyCode.W)&&chatBlock && canInput) {
            z += 1.0f;
        }

        if(Input.GetKeyDown(KeyCode.W)&&!chatBlock && canInput){
            DialogueUI.scrollY(-1);
        }

        if (Input.GetKeyDown("left shift")&&!(dialogueUI.enabled&&npcType==NpcNavMesh.NpcType.Proximity) && canInput) {
            if(forcedDown){
                forcedDown = false;
            }else{
                playerModel.SetActive(false);
                playerController.height=1;
                groundCheck.position+=positionChange;
                cam.position-=positionChange;
                isCrouch = true;
                playerSpeed = 3.0f;
            }
        }

        if (!forcedDown&&Input.GetKeyUp("left shift")&&!(dialogueUI.enabled&&npcType==NpcNavMesh.NpcType.Proximity)) {
            playerController.height=2;
            if(Physics.CheckSphere(headCheck.position, headDistance, terrainMask)){
                playerController.height=1;
                forcedDown = true;
            }else{
                playerModel.SetActive(true);
                cam.position+=positionChange;
                groundCheck.position-=positionChange;
                forcedDown = false;
                isCrouch = false;
                playerSpeed = 6.0f;
            }
            
        }

        if(forcedDown){
            if(!Physics.CheckSphere(headCheck.position, headDistance, terrainMask)){
                playerModel.SetActive(true);
                playerController.height=2;
                cam.position+=positionChange;
                groundCheck.position-=positionChange;
                forcedDown = false;
                isCrouch = false;
                playerSpeed = 6.0f;
                
            }
        }
        Vector3 moveVector = transform.right * x + transform.forward * z;
        playerController.Move(moveVector * playerSpeed * Time.deltaTime);
        playerController.Move(velocity * Time.deltaTime);

        if(player.transform.position.y<-20){
            player.transform.position = spawnpoint.position;
        }

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f) {
            if (isMoving) return;
            else {
                isMoving = true;
                StopAllCoroutines();
                StartCoroutine(HeadBob(25));
            }
        } else {
            if (isMoving) {
                isMoving = false;
                StopAllCoroutines();
                StartCoroutine(Idle(0.1f));
            }
            else return;
        }
    }

    void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }

    public static void optionsOpen(){
        canMove = false;
    }

    public static void optionsClose(){
        canMove = true;
    }

    public static void notebookOpen(){
        canInput = false;
    }

    public static void notebookClose(){
        canInput = true;
    }

    IEnumerator HeadBob(float frequency) {
        float angle = 0.0f;
        while (true) {
            while (angle < 2 * Mathf.PI) {
                cam.position = new Vector3(root.position.x, root.position.y + Mathf.Sin(angle)/15.0f, root.position.z);
                angle += (2 * Mathf.PI/frequency);
                yield return null;
            }
            angle = 0.0f;
        }
    }

    IEnumerator Idle(float end) {
        float t = 0.0f;
        while (t < end) {
            cam.position = Vector3.Lerp(cam.position, root.position, t);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
