using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class BossAttackBox : MonoBehaviour
{

    [HideInInspector] public PlayerMain playerMain;
    private Boss1Manager boss;


    void Start()
    {
        boss = GetComponentInParent<Boss1Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.gameObject.GetComponent<SpriteRenderer>().flipX)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!boss.inSkillUse)
            {
                PlayerMain.Instance.PlayerData.healthSystem.Damage(200);
                Debug.Log("200 damage");
            }
            else if (boss.charging)
            {
                PlayerMain.Instance.PlayerData.healthSystem.Damage(300);
                Debug.Log("300 damage");
            }
        }
    }
}
