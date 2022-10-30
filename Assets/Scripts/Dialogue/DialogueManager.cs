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
    int activeMessage = 0;
    public static bool isActive = false;
    private Coroutine displayLineCoroutine;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        Debug.Log("Started Conversation Loaded Messages: " + messages.Length);
        DisplayMessage();
    }
    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        //messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;
        displayLineCoroutine = StartCoroutine(DisplayLine(messageToDisplay.message));
    }

    public void NextMessage()
    {
        activeMessage++;
        if(activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversation ended");
            isActive = false;
	        gameObject.SetActive(false);
            //FIXME:
	        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerInputActions.Enable();
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
                NextMessage();
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
