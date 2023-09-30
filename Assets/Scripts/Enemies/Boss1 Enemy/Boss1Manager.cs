using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Manager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    [HideInInspector] public Transform Target;
    private EnemyHealthSystem healthSystem;
    private Animator anim;
    private Rigidbody2D rigid;
    public GameObject dashSmoke;

    public GameObject smallThrowable;
    public GameObject middleThrowable;
    public GameObject bigThrowable;


    public float moveSpeed;
    private bool notDead;
    public bool canAttack;
    private bool InAnimation;
    [HideInInspector] public bool stunned;
    [HideInInspector] public bool charging;

    public float setmeleeWaitTime;
    private float meleeWaitTime;
    public float meleerange;
    public float jumpRange;

    [Header("Skills")]

    [HideInInspector] public bool inSkillUse;

    public int rageStatus;

    //charge
    public float setchargeSkillTime;
    private float chargeSkillTime;
    [HideInInspector] public bool backingUpTimer;
    private Vector2 chargingDir;


    //jump
    public float setCount;
    private float count;
    public float setjumpSkillTime;
    private float jumpSkillTime;
    public float jumpForce;
    private bool onAir;
    private Vector2 directionJump;
    private Vector2 controlPoint;
    


    //throw
    private GameObject willThrow;
    private bool gotoItem;

    //disable attack hitbox if non damage move

    public bool TEST;

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<EnemyHealthSystem>();

        TEST = false;

        notDead = true;
        canAttack = false;
        InAnimation = false;
        charging = false;
        inSkillUse = false;
        backingUpTimer = false;
        gotoItem = false;

        rageStatus = 0;

        chargeSkillTime = setchargeSkillTime;
        meleeWaitTime = setmeleeWaitTime;
    }



    private void FixedUpdate()
    {
        if (!stunned && !charging && !gotoItem)
        {
            if (canAttack && Player.transform.position.x - transform.position.x < 0)
            {
                dashSmoke.GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                dashSmoke.GetComponent<SpriteRenderer>().flipX = true;
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
                  
        //                               %70                                                       %50                                                         %32
        if (((((healthSystem.currentHealth <= 42000f && rageStatus < 0) || (healthSystem.currentHealth <= 30000f && rageStatus < 1) || (healthSystem.currentHealth <= 18000f && rageStatus < 2)) && rageStatus <= 3) || TEST) && !inSkillUse)
        {
            inSkillUse = true;
            TEST = false;
            rageStatus += 1;
            WhatToThrow();
        }




        //timers

        if (meleeWaitTime > 0)
            meleeWaitTime -= Time.deltaTime;

        if (chargeSkillTime > 0)
            chargeSkillTime -= Time.deltaTime;

        if (jumpSkillTime > 0)
            jumpSkillTime -= Time.deltaTime;
    }


    void Update()
    {
        if (notDead && !stunned)
        {

            if (canAttack)  //player in range
            {
                //skill stuff

                if (backingUpTimer)
                {
                    Vector2 directionTarget = Target.position - transform.position;
                    rigid.MovePosition((Vector2)transform.position + (directionTarget * moveSpeed * 20 * Time.deltaTime));
                }

                if (gotoItem)
                {
                    Vector2 directionItem = willThrow.transform.position - transform.position;
                    rigid.MovePosition((Vector2)transform.position + (directionItem * 3 * moveSpeed * Time.deltaTime));
                }


                if (onAir)
                {
                    if (count < setCount)
                    {
                        count += Time.deltaTime;

                        Vector3 m1 = Vector3.Lerp(transform.position, controlPoint, count);
                        Vector3 m2 = Vector3.Lerp(controlPoint, directionJump, count);
                        rigid.MovePosition(Vector3.Lerp(m1, m2, count));
                    }




                    /*
                    if (gettingHigh)
                    {
                        rigid.MovePosition((Vector2)transform.position + ((Vector2)rigid.transform.up * jumpForce * 15 * Time.deltaTime));
                    }
                    else
                    {
                        rigid.MovePosition((Vector2)transform.position + (directionJump * moveSpeed * 5 * Time.deltaTime));
                    }
                    */
                }

                if (charging)
                {
                    float testingIfDamaged = Player.GetComponent<HealthSystem>().currentHealth;
                    if (testingIfDamaged > Player.GetComponent<HealthSystem>().currentHealth)
                    {
                        anim.Play("flex");
                        charging = false;
                    }

                    rigid.MovePosition((Vector2)transform.position + (chargingDir * moveSpeed * 5 * Time.deltaTime));
                }

                //moveset

                if (!inSkillUse)
                {
                    //how far is the player

                    float distance = Vector2.Distance(Player.transform.position, transform.position);


                    if (Player.GetComponent<HealthSystem>().currentHealth <= 0)         // for short Boss will be in a special scene so if player dies they restart the battle(SCENE)
                        anim.Play("flex");                                              // so no need to restart boss's AI
                    else if (distance < meleerange)
                    {
                        if (meleeWaitTime <= 0)
                        {
                            //attackup/down
                            anim.Play("attackup");
                            meleeWaitTime = setmeleeWaitTime;
                        }
                        else if (!InAnimation)
                        {
                            anim.Play("idle");       //this can be placed with another move so he doesnt wait on our head
                        }
                    }
                    else if (jumpRange < distance && !InAnimation && jumpSkillTime <= 0)
                    {
                        StartCoroutine(Jump());
                        jumpSkillTime = setjumpSkillTime;
                    }
                    else if (distance > meleerange && !InAnimation)
                    {
                        if (!InAnimation && (chargeSkillTime <= 0))
                        {
                            StartCoroutine(Charge());
                            chargeSkillTime = setchargeSkillTime;
                        }
                        else
                        {
                            anim.Play("move");

                            Vector2 direction = Player.transform.position - transform.position;
                            rigid.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
                        }
                    }

                }

            }
            else      //not able to attack
            {
                anim.Play("idle");
            }

        }
    }


    IEnumerator Charge()
    {
        inSkillUse = true;
        anim.Play("backstep");
        dashSmoke.SetActive(true);

        yield return new WaitForSeconds(0.1f);   //giving time for the target set for boss
        backingUpTimer = true;

        yield return new WaitForSeconds(1.5f);

        backingUpTimer = false;
        anim.Play("willcharge");

        yield return new WaitForSeconds(1.5f);

        dashSmoke.SetActive(false);
        chargingDir = Player.transform.position - transform.position;
        charging = true;
        anim.Play("charge");
    }

    
    IEnumerator Jump()
    {
        inSkillUse = true;
        anim.Play("willjump");
        yield return new WaitForSeconds(1.5f);

        anim.Play("attackjump");

        rigid.constraints -= RigidbodyConstraints2D.FreezePositionY;

        rigid.AddForce(transform.up * jumpForce);

        //directionJump = new Vector3(Player.transform.position.x, Player.transform.position.y - 10, Player.transform.position.z) - transform.position;
        directionJump = new Vector2(Player.transform.position.x, Player.transform.position.y - 5f);


        controlPoint = ((transform.position + Player.transform.position) / 2f) + new Vector3(0f,15f,0f);

        /*
        if (GetComponent<SpriteRenderer>().flipX)
        {
            controlPoint = new Vector2(transform.position.x - 7f, transform.position.y + 9f);
        }
        else
        {
            controlPoint = new Vector2(transform.position.x + 7f, transform.position.y + 9f);
        }
        */

        count = 0f;
        onAir = true;

        yield return new WaitForSeconds(1.2f); //do it better 

        rigid.velocity = Vector3.zero;


        onAir = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        rigid.constraints -= RigidbodyConstraints2D.FreezePositionX;

        chargeSkillTime += 5f;

        yield return new WaitForSeconds(1.5f); //do it better 
        inSkillUse = false;
    }

    void WhatToThrow()
    {
        switch (rageStatus)
        {
            case 1:
                willThrow = smallThrowable;
                break;

            case 2:
                willThrow = middleThrowable;
                break;

            case 3:
                willThrow = bigThrowable;
                break;
        }
        StartCoroutine(Throwing());
    }

    public IEnumerator Throwing()
    {

        yield return new WaitForSeconds(1f);  //for calculating idk

        gotoItem = true;

        yield return new WaitUntil(() => (transform.position.x - willThrow.transform.position.x) < 1f);

        gotoItem = false;

        //animator pick up item
        anim.Play("flex");
        yield return new WaitForSeconds(1f);

        //animator throw item

        willThrow.GetComponent<BossThrowableObject>().ThrowItem(Player);

        yield return new WaitForSeconds(2f);
        inSkillUse = false;
    }

    public IEnumerator Stun()
    {
        charging = false;
        stunned = true;

        anim.Play("chargefail");
        yield return new WaitForSeconds(4f);
        inSkillUse = false;
        meleeWaitTime = setmeleeWaitTime;
        stunned = false;
    }

    public void AnimationTime(int answer)
    {
        if (answer > 0)
            InAnimation = true;
        else
            InAnimation = false;
            
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, jumpRange);
        Gizmos.DrawWireSphere(transform.position, meleerange);
    }
}
