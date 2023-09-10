using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public Boss1Manager boss1Manager;
    void Start()
    {
        boss1Manager = GetComponentInParent<Boss1Manager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            boss1Manager.Player = collision.gameObject;
            boss1Manager.canAttack = true;
        }

        if (collision.tag == "wall")
        {
            boss1Manager.stunned = true;
        }
    }
}
