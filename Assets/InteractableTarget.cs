using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTarget : MonoBehaviour
{
    public int hitpoints = 3; // Public member to track hitpoints

    private void OnCollisionEnter(Collision collision) // Detect collisions
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Projectile")) // Check for "Projectile" tag
        {
            hitpoints--; // Decrement hitpoints
            Destroy(collision.gameObject); // Destroy the colliding object

            if (hitpoints <= 0) // Check if hitpoints are 0 or less
            {
                Destroy(gameObject); // Destroy this object
            }
        }
    }
}