using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;
    public bool showGroundDetection = true;
    public float playerSpeed = 1.5f;
    public float jumpHeight = 5.0f;
    public float gravity = 9.8f;
    public float groundDistance = 0.4f;
    public float headDistance = 0.4f;
    public LayerMask terrainMask;
    public CharacterController playerController;
    public Transform groundCheck;
    public Transform headCheck;
    public Transform cam;
    Vector3 velocity;
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
        playerModel = GameObject.Find("Player");
        if (showGroundDetection) {
            groundDectector = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            groundDectector.name = "Debug Ground Detector";
            groundDectector.transform.parent = transform;
        }
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

        if (Input.GetButtonDown("Jump") && isGrounded) {
            Jump();
        }

        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) {
            x += -1.0f;
        }

        if (Input.GetKey(KeyCode.D)) {
            x += 1.0f;
        }

        if (Input.GetKey(KeyCode.S)) {
            z += -1.0f;
        }

        if (Input.GetKey(KeyCode.W)) {
            z += 1.0f;
        }

        if (!forcedDown&&Input.GetKeyDown("left shift")) {
            playerModel.SetActive(false);
            playerController.height=1;
            groundCheck.position+=positionChange;
            cam.position-=positionChange;
            isCrouch = true;
            playerSpeed = 3.0f;
        }

        if (!forcedDown&&Input.GetKeyUp("left shift")) {
            playerController.height=2;
            if(Physics.CheckSphere(headCheck.position, headDistance, terrainMask)){
                playerController.height=1;
                forcedDown = true;
            }else{
                cam.position+=positionChange;
                groundCheck.position-=positionChange;
                forcedDown = false;
                isCrouch = false;
                playerSpeed = 6.0f;
            }
            
        }

        if(forcedDown){
            if(!Physics.CheckSphere(headCheck.position, headDistance, terrainMask)){
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
    }

    void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }
}
