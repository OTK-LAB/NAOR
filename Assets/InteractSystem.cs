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

    Rigidbody2D related_rigidbody; 
    Animator anim;
    Animator anim2;
    Transform PlayerPosition;
    bool active = false;
    Vector2 movement;

    void Start()
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        movement =new Vector2(PlayerPosition.position.x - 1, PlayerPosition.position.y);
    }
    void Update()
    {
        tagControl(isInRange);
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
        else if(this.tag=="boxcontrol")
        {
            related_rigidbody = related_object.GetComponent<Rigidbody2D>();

            if (isInRange && Input.GetKeyDown(interactKey) && !active) // etki alanýndaysa ve doðru tuþa basmýþsa
                active = true; //hareketi aktifleþtir player scripti enable false
            else if (isInRange && Input.GetKeyDown(interactKey) && active) // etki alanýndaysa, doðru tuþa bir kez daha basmýþsa
                active = false; //hareketi iptal et player scripti enable true

            if (isInRange && active) //etki alanýndaysa ve hareket yeteneðini aktif etmiþse iliþkili event fonksiyonunu çaðýr yani boxcontrol
                interactAction.Invoke();
        }
        else if(this.tag=="music")
        {
            if (isInRange && Input.GetKeyDown(interactKey))
                interactAction.Invoke();
           
        }
        else if(this.tag=="arrowtrigger")
        {
            if(isInRange && !active)
            {
                active = true;
                //related_object.SetActive(true);
                interactAction.Invoke();
            }
        }
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
        anim2.SetBool("trig", true);
        anim.SetBool("Trigger", true);

        StartCoroutine(backtoIdle());
    }
    IEnumerator backtoIdle()
    {
        yield return new WaitForSeconds(4);
        anim.SetBool("Trigger", false);
        anim2.SetBool("trig", false);
        active = false;
        isInRange = false;
    }
    public void BoxControl()
    {
        Debug.Log("Doðru tuþ, hareket etcem");
    
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity üzerinden kapatýyoruz zaten hareket etmemiþ oluyo 
        movement.y= Input.GetAxisRaw("Vertical");
        related_rigidbody.MovePosition(related_rigidbody.position + movement * 0.5f * Time.fixedDeltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = false;
    }
}
