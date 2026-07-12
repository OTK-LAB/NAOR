using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman_Run : StateMachineBehaviour
{
    const string attack = "Spearman_attack";
    const string idle = "Spearman_idle";
    public float attackRange;
    public float speed = 1f;

    private bool isFlipped = false;

    Spearman_Manager spear;
    Transform player;
    Rigidbody2D rb;
    Vector3 movement;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        spear = animator.GetComponent<Spearman_Manager>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spear.playerNear)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newpos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(attack))
                LookAtPlayer();

            if (!spear.shieldBroke)
                spear.shieldcoll.SetActive(true);


            if (Vector2.Distance(player.position, rb.position) <= attackRange)
            {

            }

            if (Vector2.Distance(player.position, rb.position) <= attackRange)
            {
                spear.inRange = true;
                if (spear.attackTimer <= 0)
                {
                    animator.Play(attack);
                    spear.attackTimer = spear.autoAttackTimer;                      //attacks are checked from the animation events
                }
                else
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName(attack))
                    {
                        animator.Play(idle);
                        LookAtPlayer();
                    }
                }
            }
            else
            {
                spear.inRange = false;
                rb.MovePosition(newpos);
            }
        }
        else
        {
            CheckAttack();
            AutoMove();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spear.shieldcoll.SetActive(false);
    }

    public void LookAtPlayer()
    {

        if (rb.transform.position.x < player.position.x && isFlipped)
        {

            rb.transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (rb.transform.position.x > player.position.x && !isFlipped)
        {

            rb.transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    void AutoMove()
    {
        if (!spear.playerNear)
        {
            if (spear.Moveright)
            {
                movement = new Vector3(speed, 0f, 0f);
                rb.transform.position = rb.transform.position + movement * Time.deltaTime;
            }
            else
            {
                movement = new Vector3(-speed, 0f, 0f);
                rb.transform.position = rb.transform.position + movement * Time.deltaTime;
            }
        }
    }

    void flip()
    {
        if (player.position.x > (rb.transform.position.x + 0.5f))
        {
            if (!spear.Moveright)
            {
                rb.transform.Rotate(0f, 180f, 0f);
                spear.Moveright = true;
            }
        }
        else
        {
            if (spear.Moveright)
            {
                rb.transform.Rotate(0f, 180f, 0f);
                spear.Moveright = false;
            }
        }
    }
    void CheckAttack()
    {
        if (Vector2.Distance(rb.transform.position, player.position) <= spear.triggerRange)
        {
            spear.playerNear = true;
        }
        else
            spear.playerNear = false;
    }
}
