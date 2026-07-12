using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private Sword_Behaviour enemyParent;
    private bool inRange;
    private Animator anim;

    private Rigidbody2D rigidbdy;

    private void Awake()
    {
        rigidbdy = GetComponentInParent<Rigidbody2D>();
        enemyParent = GetComponentInParent<Sword_Behaviour>();
        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            enemyParent.Flip();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
            enemyParent.triggerArea.SetActive(false);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyParent.triggerArea.SetActive(true);
            enemyParent.SelectTarget();
            enemyParent.inRange = false;
            gameObject.SetActive(false);
            inRange = false;
        }
    }
}
