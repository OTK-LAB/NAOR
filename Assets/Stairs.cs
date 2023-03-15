using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private void Start()
    {
        // Get the BoxCollider2D component of the stairs object
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            boxCollider.enabled = false;
        }
    }
}
