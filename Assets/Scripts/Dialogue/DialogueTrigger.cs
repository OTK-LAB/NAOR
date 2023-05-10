using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class DialogueTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actors;
    public GameObject DialogueBox;
    public GameObject InteractionText;
    public Animator animator;
    public PlayerInputActions inputActions;
    public bool inRange = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.NpcInteraction.started += OnDialougeTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
	    if (collider.CompareTag("Player") && InteractionText != null)
	    {   
            inRange = true;
	        InteractionText.SetActive(true);
	    }
    }
    
     private void OnTriggerExit2D(Collider2D collider)
     {
        if (collider.CompareTag("Player") && InteractionText != null)
        {
            inRange = false;
            InteractionText.SetActive(false);
            inputActions.Interaction.Enable();
        }
     }

    public void StartDialogue()
    {
        //FIXME:
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerInputActions.Disable();
        Destroy(InteractionText);
	    DialogueBox.SetActive(true);
        FindObjectOfType<DialogueManager>().OpenDialogue(messages, actors);
	    inputActions.Interaction.Disable();
    }

    void OnDialougeTriggered(InputAction.CallbackContext context)
    {
        if(inRange)
        {
            StartDialogue();
        }
    }
}

[System.Serializable]
public class Message
{
    public int actorId;
    public string message;
}

[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}
