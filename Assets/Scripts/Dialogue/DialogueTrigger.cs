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

    public void StartDialogue()
    {
	DialogueBox.SetActive(true);
        FindObjectOfType<DialogueManager>().OpenDialogue(messages, actors);
	
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
	if (collider.CompareTag("Player") == true)
	{
	    InteractionText.SetActive(true);
	}
    }
    
     private void OnTriggerExit2D(Collider2D collider)
     {
      InteractionText.SetActive(false);
     }

    private void Update()
    {
	if (InteractionText.activeInHierarchy)
	{
		if(Mouse.current.leftButton.wasPressedThisFrame && !DialogueBox.activeInHierarchy)
	    	{
	    	 //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().enabled = false;
	         GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	    	 StartDialogue();
		}
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
