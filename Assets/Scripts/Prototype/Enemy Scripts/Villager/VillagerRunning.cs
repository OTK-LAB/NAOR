using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerRunning : MonoBehaviour
{
    private bool playerDetected;
    public float detectedAreaRadius;

    public LayerMask WhatIsPlayer;
    
    public float speed;

    private Transform playerPos;
    
    public bool facingRight = true;

    private int b = 0;

    private bool isGrounded;
    public Transform groundDetection;
    public float distance;
    public LayerMask WhatIsGround;
    private Animator animator;
    private string currentState;
    const string idle = "VillagerIdle";
    const string run = "VillagerRun";





    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputDirection();
        Detected();
    }

    void CheckInputDirection()
    {
        if (playerPos.position.x > gameObject.transform.position.x && facingRight) 
            Flip();
        if (playerPos.position.x < gameObject.transform.position.x && !facingRight)
            Flip();
    }

    void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }


    void RunAway()
    {
        if (playerPos.position.x > gameObject.transform.position.x )
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        if (playerPos.position.x < gameObject.transform.position.x )
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectedAreaRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundDetection.position, distance);
    }

    void Detected()
    {
        playerDetected = Physics2D.OverlapCircle(gameObject.transform.position, detectedAreaRadius, WhatIsPlayer);

        isGrounded = Physics2D.OverlapCircle(groundDetection.transform.position, distance, WhatIsGround);

        if (isGrounded == true)
        {
            if (playerDetected == true || b == 1)
            {
                RunAway();
                ControlOn();
                if(!GetComponent<VillagerHealthManager>().isHurting)
                    ChangeAnimationState(run);
            }
            else if (playerDetected == false && b == 1)
            {
                RunAway();
                if(!GetComponent<VillagerHealthManager>().isHurting)
                    ChangeAnimationState(run);
            }

            else if (playerDetected == false)
            {
                if(!GetComponent<VillagerHealthManager>().isHurting)
                    ChangeAnimationState(idle);
            }
        }

    }
    void ChangeAnimationState(string newState)
    {
        if(currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    void ControlOn()
    {
        b = 1;
    }

}
