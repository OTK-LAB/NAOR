using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{

    public GameObject OtherBossCharge;

    public Boss1Manager boss1Manager;

    public bool playerDetector;
    void Start()
    {
        if (boss1Manager == null)
        {
            boss1Manager = GetComponentInParent<Boss1Manager>();
        }

        if (boss1Manager == null)
        {
            Debug.Log("Please put boss's boss1manager script into the BossChargePoints");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerDetector)
        {
            if (collision.tag == "Player")
            {
                boss1Manager.Player = collision.gameObject;
                boss1Manager.canAttack = true;
            }

            if (collision.tag == "wall" && boss1Manager.charging)
            {
                StartCoroutine(boss1Manager.Stun());
            }
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerDetector)
        {
            if (collision.tag == "Player" && boss1Manager.inSkillUse && !boss1Manager.backingUpTimer)
            {
                boss1Manager.Target = OtherBossCharge.transform;
            }
        }

    }
}

