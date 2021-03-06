using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static bool canLook = true;

    public float mouseSensitivity = 100f;

    GameObject player;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("First Person Player");
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
void FixedUpdate() 
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseX);
    }

    public static void uiOpen(){
        canLook = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void uiClose(){
        canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
