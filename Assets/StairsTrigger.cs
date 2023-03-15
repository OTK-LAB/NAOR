using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private bool isPlayerOnStairs = false;
    public BoxCollider2D boxCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnStairs = true;
        }
        */
    }
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player is jumping (by checking the y velocity)
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0 && !isPlayerOnStairs)
            {
                // Activate the box collider when the player jumps on the stairs
                boxCollider.enabled = true;
            }
            else if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0 && isPlayerOnStairs)
            {
                // Deactivate the box collider when the player is walking normally
                boxCollider.enabled = false;
            }
        }
    }

    
}
