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
    bool active = false;
    Vector2 movement;

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

            if (isInRange && Input.GetKeyDown(interactKey) && !active) // etki alan�ndaysa ve do�ru tu�a basm��sa
                active = true; //hareketi aktifle�tir player scripti enable false
            else if (isInRange && Input.GetKeyDown(interactKey) && active) // etki alan�ndaysa, do�ru tu�a bir kez daha basm��sa
                active = false; //hareketi iptal et player scripti enable true

            if (isInRange && active) //etki alan�ndaysa ve hareket yetene�ini aktif etmi�se ili�kili event fonksiyonunu �a��r yani boxcontrol
                interactAction.Invoke();
        }
    }
    public void BoxControl()
    {
        Debug.Log("Do�ru tu�, hareket etcem");
    
        movement.x = Input.GetAxisRaw("Horizontal"); //x'i unity �zerinden kapat�yoruz zaten hareket etmemi� oluyo 
        movement.y= Input.GetAxisRaw("Vertical");
        related_rigidbody.MovePosition(related_rigidbody.position + movement * 0.5f * Time.fixedDeltaTime);

    }
}
