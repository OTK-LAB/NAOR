using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Manager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    private Animator anim;
    private Rigidbody2D rigid;

    public float moveSpeed;
    private bool notDead;
    public bool canAttack;
    private bool InAnimation;
    [HideInInspector] public bool stunned;

    public float setmeleeWaitTime;
    private float meleeWaitTime;
    public float meleerange;

    //disable attack hitbox if non damage move

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();



        notDead = true;
        canAttack = false;
        InAnimation = false;


    }


    private void FixedUpdate()
    {
        if (canAttack && Player.transform.position.x - transform.position.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

        if (meleeWaitTime > 0)
        {
            meleeWaitTime -= Time.deltaTime;
        }
    }



    void Update()
    {
        if (notDead)
        {

            if (stunned)
            {

            }



            if (canAttack)
            {
                //how far is the player

                float distance = Vector2.Distance(Player.transform.position, transform.position);

                //moveset

                if (distance < meleerange)
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
                else if (distance > meleerange)
                {







                        //if else en sonu
                        
                        anim.Play("move");

                        Vector2 direction = Player.transform.position - transform.position;
                        rigid.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
                }






                if (Player.GetComponent<HealthSystem>().currentHealth <= 0)        // for short Boss will be in a special scene so if player dies they restart the battle(SCENE)
                    anim.Play("flex");                                             // so no need to restart boss's AI
            }
            else      //not able to attack
            {
                anim.Play("idle");
            }
        }
    }

    public void AnimationTime(int answer)
    {
        if (answer > 0)
            InAnimation = true;
        else
            InAnimation = false;
            
    }
}
