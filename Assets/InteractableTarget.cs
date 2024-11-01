using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractableTarget : MonoBehaviour
{
    public ParticleSystem hitEffect;
    public int hitpoints = 3; // Public member to track hitpoints

    private void OnCollisionEnter2D(Collision2D collision) // Detect collisions
    {
        //Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Projectile")) // Check for "Projectile" tag
        {
            hitpoints--; // Decrement hitpoints
            //transform.DOShakePosition(0.1f, 0.1f, 10, 90);
            hitEffect.Play(); 
            Destroy(collision.gameObject); // Destroy the colliding object

            if (hitpoints <= 0) // Check if hitpoints are 0 or less
            {
                Destroy(gameObject); // Destroy this object
            }
        }
    }
}