using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EffectBombScript : MonoBehaviour
{
    public bool Destroyed = false;
    public Item item;

    private float timer;
    private void Awake()
    {
    }
    void Start()
    {
        timer = 6;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer<=0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //slow down enemy movoment speed and attack speed
                col.GetComponent<EnemyController>().speedReduction(col,4);
            }
    }
}
