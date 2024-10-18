using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LineRenderer))]
public class Character : MonoBehaviour
{
    public float speed = 5f; // Speed modifier
    public Transform target; // Target GameObject to rotate towards 
    public Vector3 rotationOffset; // Offset for the look rotation
    public float lineLength = 5f;
    private CharacterController controller; // Character Controller component
    private LineRenderer lineRenderer;


    void Start()
    {
        controller = GetComponent<CharacterController>(); // Initialize the Character Controller
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveVertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        // Create a movement vector
        Vector3 move = new Vector3(moveHorizontal, moveVertical, 0);

        // Move the character
        controller.Move(move * speed * Time.deltaTime);

        // Rotate towards the target
        if (target != null) // Check if target is assigned
        {
            Vector3 direction = (target.position - transform.position).normalized; // Calculate direction to target
            if (direction != Vector3.zero) // Ensure direction is not zero
            {
                // Calculate the angle to rotate only on the Z-axis
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset.z; // Get angle in degrees
                Quaternion lookRotation = Quaternion.Euler(0, 0, angle); // Create rotation only on Z-axis
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed); // Smoothly rotate towards target
            }


            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position); // Start point
            lineRenderer.SetPosition(1, transform.position + transform.up * lineLength);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

}
