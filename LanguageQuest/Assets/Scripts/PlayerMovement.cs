using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;
    public float playerSpeed = 1.5f;
    public float jumpHeight = 5.0f;
    public float gravity = 9.8f;
    public float groundDistance = 0.4f;
    public float headDistance = 0.4f;

    public LayerMask terrainMask;
    public CharacterController playerController;
    public Transform groundCheck;
    public Transform headCheck;

    Vector3 velocity;
    float xTotal = 0.0f;
    float zTotal = 0.0f;
    bool isGrounded;
    bool hitHead;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, terrainMask);
        hitHead = Physics.CheckSphere(headCheck.position, headDistance, terrainMask);
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

        if (Input.GetKey(KeyCode.A)) {
            xTotal += -0.02f;
        }

        if (Input.GetKey(KeyCode.D)) {
            xTotal += 0.02f;
        }

        if (Input.GetKey(KeyCode.S)) {
            zTotal += -0.02f;
        }

        if (Input.GetKey(KeyCode.W)) {
            zTotal += 0.02f;
        }

        // If W and S are both not pressed or both are pressed
        if ((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))) {
            if (zTotal > 0.0f) zTotal -= 0.02f;
            else if (zTotal < 0.0f) zTotal += 0.02f;
        }

        // If A and D are both not pressed or both are pressed
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ) ) {
            if (xTotal > 0.0f) xTotal -= 0.02f;
            else if (xTotal < 0.0f) xTotal += 0.02f;
        }

        // Resolve float rounding errors
        if (xTotal < 0.02f && xTotal > -0.02f) xTotal = 0.0f;
        if (zTotal < 0.02f && zTotal > -0.02f) zTotal = 0.0f;

        // Clamp speed
        xTotal = Mathf.Clamp(xTotal, -1.0f, 1.0f);
        zTotal = Mathf.Clamp(zTotal, -1.0f, 1.0f);

        // Apply moveVector and velocity from jumping/grav
        Vector3 moveVector = transform.right * xTotal + transform.forward * zTotal;
        playerController.Move(moveVector * playerSpeed * Time.deltaTime);
        playerController.Move(velocity * Time.deltaTime);
    }

    void Jump() {
        velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }
}
