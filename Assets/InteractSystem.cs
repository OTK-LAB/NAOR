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
    Rigidbody2D player_rb; 
    Animator anim;
    Animator anim2;
    Transform PlayerPosition;
    bool active = false;
    bool actived = false;
    Vector2 movement;

    bool deneme = true;
    void Start()
    {
    }
    void Update()
    {
        if (this.tag != "Player")
            tagControl(isInRange);
        else if (this.tag == "Player" && deneme)
            move();
    }

    public void move()
    {
        player_rb = this.gameObject.GetComponent<Rigidbody2D>();
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity üzerinden kapatýyoruz zaten hareket etmemiþ oluyo 
        movement.y = Input.GetAxisRaw("Vertical");
        player_rb.MovePosition(player_rb.position + movement * 5f * Time.fixedDeltaTime);

    }
    void tagControl(bool isInRange)
    {
        /*
        if (this.tag == "stairs")
        {
            anim = related_object.GetComponent<Animator>();
            if (isInRange)
                stairsActive();
            else
                stairsCancel();
        }
        */
       
        if(this.tag == "platform")
        {
            Debug.Log("Platformmuþ bu"); 
            anim2 = related_object2.GetComponent<Animator>();
            
            if (isInRange && !actived)
                platformActive();
        }
       
        else if(this.tag=="boxcontrol")
        {
            related_rigidbody = related_object.GetComponent<Rigidbody2D>();

            if (isInRange && Input.GetKeyDown(interactKey) && !active)                  // etki alanýndaysa ve doðru tuþa basmýþsa
                active = true;          //hareketi aktifleþtir player scripti enable false
            else if (isInRange && Input.GetKeyDown(interactKey) && active)              // etki alanýndaysa, doðru tuþa bir kez daha basmýþsa
                active = false;         //hareketi iptal et player scripti enable true

            if (isInRange && active)             //etki alanýndaysa ve hareket yeteneðini aktif etmiþse iliþkili event fonksiyonunu çaðýr yani boxcontrol
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
        Debug.Log("Çaðýrýldý");
        interactAction.Invoke();
        actived = true;
        anim2.SetBool("trig", true);
        

        StartCoroutine(backtoIdle());
    }
    IEnumerator backtoIdle()
    {
        yield return new WaitForSeconds(14);
        
        anim2.SetBool("trig", false);
        actived = false;
        isInRange = false;
    }
    public void BoxControl()
    {
        Debug.Log("Doðru tuþ, hareket etcem");
        deneme = false;
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity üzerinden kapatýyoruz zaten hareket etmemiþ oluyo 
        movement.y= Input.GetAxisRaw("Vertical");
        related_rigidbody.MovePosition(related_rigidbody.position + movement * 0.5f * Time.fixedDeltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = true;
        if (collision.gameObject.CompareTag("box") && this.tag == "stairs")
            isInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = false;
    }
}
