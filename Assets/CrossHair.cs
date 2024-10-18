using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    private bool stickToCursor = false;

    [SerializeField]
    private float movementDelay = 0.1f; // Delay for movement

    private Vector3 targetPosition;

    void Update()
    {
        if (stickToCursor)
        {
            StickToCursor();
        }
    }

    private void StickToCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane; // Set the distance from the camera
        targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        targetPosition.z = transform.position.z; // Keep the original z position

        // Smoothly move towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementDelay);
    } 
}
