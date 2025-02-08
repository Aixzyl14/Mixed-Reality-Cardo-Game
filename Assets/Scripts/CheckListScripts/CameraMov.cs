using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMov : MonoBehaviour
{
    public float mouseSensitivity = 800f; // Speed of mouse movement
    private float xRotation = 0f;         // Keep track of camera's vertical rotation
    public Transform playerBody;  // Reference to the player's body (if you want the camera to follow the player)

    public PlayerMov Player;
    private bool EnableCamera;
    private bool CursorLock = true;

    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponentInParent<PlayerMov>();
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        EnableCamera = Player.EnableMovement;
        if (EnableCamera)
        {
            MouseLook();
            CursorLock = true;
        }
        else
        {
            // Unlock the cursor so you can input to UI
            Cursor.lockState = CursorLockMode.None;
            CursorLock = false;
        }
        if (CursorLock)
        { 
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        }
    }


    // Function to move the camera based on the mouse movement
    void MouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust vertical rotation (look up/down), clamping it to avoid over-rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Prevent the camera from flipping over

        // Apply the vertical (up/down) rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal (left/right) rotation to the player's body or the camera's Y-axis
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
