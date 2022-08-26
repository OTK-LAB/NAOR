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
    public GameObject related_cp;
    public GameObject easingEditor;
    GameObject player;

    Rigidbody2D related_rigidbody; 
    Rigidbody2D player_rb; 
    Animator anim;
    
    bool active = false;
    [HideInInspector] public bool arrowHit = false;
    bool playerMovement = true;

    Vector2 movement;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (this.tag != "Player")
            tagControl(isInRange);
        else if (this.tag == "Player" && playerMovement)
            move();
        if (arrowHit)
        {
            Debug.Log("reborn");
            GameObject.FindGameObjectWithTag("darkness").GetComponent<Animator>().enabled = true;
            arrowHit = false;
            active = false;
            player.GetComponent<Transform>().position = movement;
        }
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

        if (this.tag == "stairs")
        {
            if (isInRange && !active)
                stairsActive();
            else if (!isInRange && active)
                stairsDeactive();

            
        }
       
        else if(this.tag == "platform")
        {  
            anim = related_object.GetComponent<Animator>();            
            if (isInRange && !active)
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
                movement = new Vector2 (related_cp.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                active = true;
                interactAction.Invoke();
            }
        }
    }
    
    void stairsActive()
    {
        interactAction.Invoke();
        active = true;
    }

    void stairsDeactive()
    {
        easingEditor.GetComponent<EasingEditor>().miniStairsCome();
        
        active = false;
    }
    void platformActive()
    {        
        interactAction.Invoke();
        active = true;
        anim.SetBool("Trigger", true);
        StartCoroutine(backtoIdle());
    }
    IEnumerator backtoIdle()
    {
        yield return new WaitForSeconds(6); // WaitForSeconds is (first move time + delay time)
        
        anim.SetBool("Trigger", false);
        active = false;
        isInRange = false;
    }
    public void BoxControl()
    {
        Debug.Log("Doðru tuþ, hareket etcem");
        playerMovement = false;
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity üzerinden kapatýyoruz zaten hareket etmemiþ oluyo 
        movement.x = related_rigidbody.position.x;
        //movement.y= Input.GetAxisRaw("Vertical");
        if ( Input.GetKeyDown(KeyCode.W))
        {
            movement.y = related_rigidbody.position.y + 5;
            related_rigidbody.position = movement;
        }
            
        else if (Input.GetKeyDown(KeyCode.S))
        {
            movement.y = related_rigidbody.position.y - 5;
            related_rigidbody.position = movement;
        }
            
      
        //related_rigidbody.MovePosition(related_rigidbody.position + movement * 0.5f * Time.fixedDeltaTime);

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
