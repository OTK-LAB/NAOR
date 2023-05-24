using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    
    public float typingSpeed = 0.04f;
    public Image actorImage;
    public  TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;

    Message[] currentMessages;
    Actor[] currentActors;

    public GameObject Buttons;
    public int buttonId;
    public int finalMessage;

    int activeMessage;
    int startingMessage;
    public static bool isActive = false;
    private Coroutine displayLineCoroutine;

    public void SetStartingMessage(int newStartingMessage)
    {
       startingMessage = newStartingMessage;
    }

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = startingMessage;
        isActive = true;
        Debug.Log("Started Conversation Loaded Messages: " + messages.Length);
        DisplayMessage();
    }
    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;
        displayLineCoroutine = StartCoroutine(DisplayLine(messageToDisplay.message));
        if (buttonId == activeMessage)
            Buttons.SetActive(true);
    }

    public void NextMessage(int messageId)
    {
        activeMessage = messageId;
        if(activeMessage < currentMessages.Length)
        {
            DisplayMessage();
            //if (buttonId == activeMessage)
               // Buttons.SetActive(true);
        }
        else
        {
            Debug.Log("Conversation ended");
            isActive = false;
	        gameObject.SetActive(false);
            //FIXME:
	        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerInputActions.Enable();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputManager>().playerControls.Enable();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        //empty the dialogue text
        messageText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if ((Mouse.current.leftButton.wasPressedThisFrame) && isActive == true)
        {
            if (messageText.text != currentMessages[activeMessage].message)
            {
                if (displayLineCoroutine != null)
                {
                    StopCoroutine(displayLineCoroutine);
                    messageText.text = currentMessages[activeMessage].message;
                }           
            }
            else
            {
                if (activeMessage != buttonId && !Buttons.activeInHierarchy)
                {
                    if (activeMessage == finalMessage)
                        NextMessage(currentMessages.Length);
                    else
                        NextMessage(activeMessage + 1);
                }
            }
        }

    }

/*	IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        if (isActive == true)
		NextMessage();
    }
*/
}
