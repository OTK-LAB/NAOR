using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InteractSystem : MonoBehaviour
{

    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public GameObject related_object;
    Animator anim;

    void Start()
    {

    }
    void Update()
    {
        if (isInRange)
            tagControl(isInRange);
        else
            tagControl(isInRange);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            isInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = false;
    }
    void stairsActive()
    {
        anim.SetBool("Trigger", true);
    }
    void stairsCancel()
    {
        anim.SetBool("Trigger", false);
    }
    void tagControl(bool isInRange)
    {
        if (this.tag == "stairs")
        {
            anim = related_object.GetComponent<Animator>();
            if (isInRange)
                stairsActive();
            else
                stairsCancel();
        }

    }

}
