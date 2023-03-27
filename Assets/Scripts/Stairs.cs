using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stairs : MonoBehaviour
{
    public GameObject boxCollider;
    public PlayerInputActions inputActions;
    bool downCalled = false;
    void Awake()
    {
        // Get the BoxCollider2D component of the stairs object

        // Set up the player input controls
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Down.started += OnDownStarted;
    }
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player is jumping (by checking the y velocity)
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0 && !downCalled)
            {
                // Activate the box collider when the player jumps on the stairs
                boxCollider.SetActive(true);
            }
            else
            {
                // Deactivate the box collider when the player is walking normally
                boxCollider.SetActive(false);
            }
        }
    }
    */
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deactivate the box collider when the player leaves the stairs
            boxCollider.SetActive(true);
            downCalled = false;
        }
    }
    
   void OnDownStarted(InputAction.CallbackContext context)
    {
        Debug.Log("OnDownStarted() called");
        if(boxCollider)
            downCalled = true;
        // Disable the box collider when the player presses the down arrow key
        boxCollider.SetActive(false);

    }
}
