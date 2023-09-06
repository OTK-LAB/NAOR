using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Manager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    private Animator anim;

    private bool notDead;
    public bool canAttack;
    private bool InAnimation;

    public float meleerange;

    void Start()
    {
        anim = GetComponent<Animator>();


        notDead = true;
        canAttack = false;
        InAnimation = false;


    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.tag == "Player")
        {
            canAttack = true;
        }
    }






    void Update()
    {
        if (notDead)
        {
            if (canAttack)
            {



                if (Player.GetComponent<HealthSystem>().currentHealth <= 0)        // for short Boss will be in a special scene so if player dies they restart the battle(SCENE)
                    anim.Play("flex");                                             // so no need to restart boss's AI
            }
            else      //not able to attack
            {
                anim.Play("idle");
            }
        }
    }
}
