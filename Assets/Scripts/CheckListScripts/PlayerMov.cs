using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMov: MonoBehaviour
{
    public float moveSpeed = 25f; // Speed of movement
    public float jumpForce = 10f; // Force applied for jumping
    private Rigidbody rb;

    public GameObject Enemies; 


    private bool isGrounded;
    public bool EnableMovement;
    public Canvas Checklist;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Checklist.gameObject.SetActive(false); //Disable Checklist at Start
        EnableMovement = true;
    }

    private void Update()
    {
        if (Input.GetKey("m"))  //Open Checklist
        {
            Checklist.gameObject.SetActive(true);
            EnableMovement = false;
        }
        if (Input.GetKey("escape"))  //Close Checklist
        {
            Checklist.gameObject.SetActive(false);
            EnableMovement = true;
        }
        if (Input.GetKey("."))
        {
            EnableMovement = true;
        }
        if (EnableMovement)
        {
            // Check if the player is grounded
            isGrounded = Physics.CheckSphere(transform.position, 0.1f, LayerMask.GetMask("Ground")); // Adjust layer mask as necessary

            // Get input for movement
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Create movement vector
            Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized * moveSpeed;

            // Convert local movement to world space based on player's rotation
            movement = transform.TransformDirection(movement);

            // Move the player
            rb.MovePosition(rb.position + movement * Time.deltaTime);

            // Jump
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        //if (Input.GetKey("e"))  // Activate enemies
        //{
        //    Enemies.gameObject.SetActive(true); 
        //}
    }
}
