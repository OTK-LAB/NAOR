using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool usingBoat;
    public float boatSpeed;
    private PlayerController player;
    private bool ePressed;
    [HideInInspector] public bool inBoat;
    private bool docksReached = true;
    private bool endDocksReached = false;
    public AudioSource music;
    public GameObject tallCollRight, tallCollLeft;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && inBoat && rb.velocity.x == 0)
        {
            ePressed = true;
            if(docksReached)
            {
                music.PlayDelayed(1);
                tallCollRight.SetActive(false);   
            }
        }

        if(music.isPlaying && music.volume < 1)
        {
            music.volume += Time.deltaTime / 10;
        }

        if(usingBoat)
        {
            if(boatSpeed < 4 && docksReached)
                boatSpeed += Time.deltaTime;
            if(boatSpeed > 0 && endDocksReached)
            {
                boatSpeed -= Time.deltaTime / 2;
                tallCollLeft.SetActive(true);
            }
            else if(boatSpeed < 0)
                boatSpeed = 0;
            rb.velocity = new Vector2(boatSpeed, rb.velocity.y);
        }
        else
            rb.velocity = new Vector2(0,0);
        transform.position =  new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(-0.5f, 0.5f, Mathf.PingPong(Time.time, 1)) * Time.deltaTime);   
        //Mathf.Lerp(-1f, 1f, Mathf.PingPong(Time.time * 2, 1));    
    }
    private void OnTriggerStay2D(Collider2D coll) 
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            if(ePressed)
            {
                if(!usingBoat)
                {
                    ePressed = false;
                    player = coll.GetComponent<PlayerController>();
                    usingBoat = true;
                    player.enabled = false;
                    coll.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                    coll.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                    coll.transform.parent = this.gameObject.transform;
                    player.ChangeAnimationState("PlayerIdle");
                    if(!player.facingRight)
                    {
                        coll.transform.localScale = new Vector3(-coll.transform.localScale.x, coll.transform.localScale.y, coll.transform.localScale.z);
                        player.facingRight = !player.facingRight;
                    }    
                }
                else
                {
                    ePressed = false;
                    usingBoat = false;
                    player.enabled = true;
                    coll.transform.parent = null;
                    coll.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }

            }
            
            
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
            inBoat = true;
        if(other.gameObject.CompareTag("DockTrigger"))
            docksReached = false;
        if(other.gameObject.CompareTag("DockTrigger2"))
            endDocksReached = true;
            

    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
            inBoat = false;
    }
}
