using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialogueTrigger : MonoBehaviour
{

    public GameObject FetchObject;
    public GameObject OldDialogue;
    public GameObject NewDialogue;
    public GameObject DialogueBox;
    [SerializeField]
    int finalText;
    [SerializeField]
    int buttonId;
    // Update is called once per frame
    void Update()
    {
        if (FetchObject.GetComponent<FetchObjectScript>().isFetched)
        {
            //OldDialogue.SetActive(false);
            Destroy(OldDialogue);
            NewDialogue.SetActive(true);
            DialogueBox.GetComponent<DialogueManager>().SetFinalMessage(finalText);
            DialogueBox.GetComponent<DialogueManager>().SetButtonId(buttonId);
            DialogueBox.GetComponent<DialogueManager>().SetStartingMessage(0);
            DialogueBox.GetComponent<DialogueManager>().activeMessage = 0;
            //Destroy(this.GetComponent<QuestDialogueTrigger>());
        }
    }
}
