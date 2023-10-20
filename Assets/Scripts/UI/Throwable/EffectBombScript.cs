using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBombScript : MonoBehaviour
{
    public bool Destroyed = false;
    public Item item;

    public bool isWind=false;

    private void Awake()
    {
    }
    void Start()
    {

    }
    void Update()
    {
        if (Destroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isWind)
        {
            
        }
        else
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //slow down enemy movoment speed and attack speed
            }

        }
        
    }
}
