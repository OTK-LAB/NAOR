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
    public bool typeWriteRunning = false;
    //public InputAction action;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;
    private Coroutine displayLineCoroutine;
/*
    private void OnEnable()
    {
        action.Enable();
    }

    private void OneDisable()
    {
        action.Disable();
    }
*/
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
            typeWriteRunning = true;
            /*if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                messageText.text = line;
                break;
            }*/
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Mouse.current.leftButton.wasPressedThisFrame) && isActive == true)
        {
            if (typeWriteRunning)
            {
                typeWriteRunning = false;
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
