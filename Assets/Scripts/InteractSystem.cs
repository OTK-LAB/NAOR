using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractSystem : MonoBehaviour
{

    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public GameObject related_object , related_object2;
    public GameObject related_cp;
    public Image image;
    public GameObject easingEditor;
    private GameObject takeArrow;
    private GameObject takeArrow2;
    GameObject player;

    Rigidbody2D related_rigidbody; 
    Rigidbody2D player_rb; 
    Animator anim;

    
    bool active = false;
   // [HideInInspector] 
    public bool arrowHit = false;
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
       
        if (arrowHit)
            ArrowHit();
    }
    void ArrowHit()
    {
        movement.x = GameObject.FindGameObjectWithTag("start").GetComponent<Transform>().position.x;
        movement.y = player.GetComponent<Transform>().position.y;
        related_object2 = GameObject.FindGameObjectWithTag("uparrow");
        Debug.Log(related_object2);
        Destroy(related_object2);
        related_object2 = GameObject.FindGameObjectWithTag("downarrow");
        Debug.Log(related_object2);
        Destroy(related_object2);
        var tempColor = image.color;
        tempColor.a = 255;
        image.color = tempColor;
        arrowHit = false;
        active = false;
        player.GetComponent<Transform>().position = movement;
        StartCoroutine(darkness());
    }
    IEnumerator darkness()
    {
        yield return new WaitForSeconds(0.75f); // WaitForSeconds is (first move time + delay time)
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        active = false;
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
        else if(this.tag=="arrowtrigger" || this.tag == "arrowtrigger2")
        {
            if(isInRange && !active)
            {
                arrowActive();
                movement = new Vector2 (related_cp.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                active = true;
          
            }
        }
        else if (this.tag == "arrowtrigger3")
        {
            if (isInRange && !active)
            {
                arrowActive2();
                movement = new Vector2(related_cp.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                active = true;
            
            }
        }
    }
    
    void arrowActive()
    {
        takeArrow = Instantiate(related_object, related_cp.transform.position, Quaternion.identity);
        takeArrow.SetActive(true);
        StartCoroutine(backtoArrow());
        Debug.Log(related_cp.transform.position);

    }
    void arrowActive2()
    {

        takeArrow = Instantiate(related_object, related_cp.transform.position, Quaternion.identity);
        takeArrow.SetActive(true);
        takeArrow2 = Instantiate(related_object2, related_cp.transform.position, Quaternion.identity);
        takeArrow2.SetActive(true);
        StartCoroutine(backtoArrow());
    }
    IEnumerator backtoArrow()
    {
        yield return new WaitForSeconds(2); // WaitForSeconds is (first move time + delay time)

        active = false;
        isInRange = false;
    }
    void stairsActive()
    {
        easingEditor.GetComponent<EasingEditor>().miniStairsGo();
        active = true;
    }

    void stairsDeactive()
    {
        easingEditor.GetComponent<EasingEditor>().miniStairsCome();
        active = false;
    }
    void platformActive()
    {
        easingEditor.GetComponent<EasingEditor>().platformController();
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<InteractSystem>().enabled = false;
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity üzerinden kapatýyoruz zaten hareket etmemiþ oluyo 
        movement.x = related_rigidbody.position.x;
        movement.y= Input.GetAxisRaw("Vertical");
        /*if ( Input.GetKeyDown(KeyCode.W))
        {
            movement.y = related_rigidbody.position.y + 5;
            related_rigidbody.position = movement;
        }
            
        else if (Input.GetKeyDown(KeyCode.S))
        {
            movement.y = related_rigidbody.position.y - 5;
            related_rigidbody.position = movement;
        }*/
            
      
        related_rigidbody.MovePosition(related_rigidbody.position + movement * 5f * Time.fixedDeltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = true;
        if (collision.gameObject.CompareTag("Box") && this.tag == "stairs")
            isInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInRange = false;
        
    }
}
