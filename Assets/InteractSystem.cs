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
    public GameObject related_object2;
    Animator anim;
    Animator anim2;
    bool active = false;

    void Start()
    {

    }
    void Update()
    {
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
    void platformActive()
    {

        active = true;
        Debug.Log("aktif" + active);
        anim.SetBool("Trigger", true);
        anim2.SetBool("Trigger", true);
        StartCoroutine(backtoIdle());
    }
    IEnumerator backtoIdle()
    {
        Debug.Log("bekliyorum");

        yield return new WaitForSeconds(4);
        anim.SetBool("Trigger", false);
        anim2.SetBool("Trigger", false);
        active = false;
        isInRange = false;
        Debug.Log("bitti" + active);
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
        else if (this.tag == "platform")
        {
            anim = related_object.GetComponent<Animator>();
            anim2 = related_object2.GetComponent<Animator>();
            
            if (isInRange && !active)
                platformActive();
        }

    }

}
