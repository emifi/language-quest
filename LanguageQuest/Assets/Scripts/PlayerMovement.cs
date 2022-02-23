using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;

    public float playerSpeed = 1.5f;

    public CharacterController playerController;

    float xTotal = 0.0f;

    float zTotal = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        
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

        if ((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))) {
            if (zTotal > 0.0f) zTotal -= 0.02f;
            else if (zTotal < 0.0f) zTotal += 0.02f;
        }

        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ) ) {
            if (xTotal > 0.0f) xTotal -= 0.02f;
            else if (xTotal < 0.0f) xTotal += 0.02f;
        }

        if (xTotal < 0.02f && xTotal > -0.02f) xTotal = 0.0f;
        if (zTotal < 0.02f && zTotal > -0.02f) zTotal = 0.0f;

        xTotal = Mathf.Clamp(xTotal, -1.0f, 1.0f);
        zTotal = Mathf.Clamp(zTotal, -1.0f, 1.0f);

        Vector3 moveVector = transform.right * xTotal + transform.forward * zTotal;
        playerController.Move(moveVector * playerSpeed * Time.deltaTime);
    }
}
