using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Manager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    [HideInInspector] public Transform Target;
    private Animator anim;
    private Rigidbody2D rigid;
    public GameObject dashSmoke;


    public float moveSpeed;
    private bool notDead;
    public bool canAttack;
    private bool InAnimation;
    [HideInInspector] public bool stunned;
    [HideInInspector] public bool charging;

    public float setmeleeWaitTime;
    private float meleeWaitTime;
    public float meleerange;

    //[Header("Skills")]

    [HideInInspector] public bool inSkillUse;

    public float setchargeSkillTime;
    private float chargeSkillTime;
    [HideInInspector] public bool backingUpTimer;
    private Vector2 chargingDir;

    //disable attack hitbox if non damage move

    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();



        notDead = true;
        canAttack = false;
        InAnimation = false;
        charging = false;
        inSkillUse = false;
        backingUpTimer = false;

        chargeSkillTime = setchargeSkillTime;
        meleeWaitTime = setmeleeWaitTime;
    }


    private void FixedUpdate()
    {
        if (!stunned && !charging)
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


        //timers

        if (meleeWaitTime > 0)
        {
            meleeWaitTime -= Time.deltaTime;
        }

        if (chargeSkillTime > 0)
        {
            chargeSkillTime -= Time.deltaTime;
        }
    }



    void Update()
    {
        if (notDead && !stunned)
        {

            if (canAttack)
            {
                //skill stuff

                if (backingUpTimer)
                {
                    Vector2 directionTarget = Target.position - transform.position;
                    rigid.MovePosition((Vector2)transform.position + (directionTarget * moveSpeed * 7 * Time.deltaTime));
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

                    if (distance < meleerange)
                    {
                        if (meleeWaitTime <= 0)
                        {
                            //attackup/down
                            anim.Play("attackup");
                            meleeWaitTime = setmeleeWaitTime + 5f;
                        }
                        else if (!InAnimation)
                        {
                            anim.Play("idle");       //this can be placed with another move so he doesnt wait on our head
                        }
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

                    if (Player.GetComponent<HealthSystem>().currentHealth <= 0)        // for short Boss will be in a special scene so if player dies they restart the battle(SCENE)
                        anim.Play("flex");                                             // so no need to restart boss's AI
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
}
