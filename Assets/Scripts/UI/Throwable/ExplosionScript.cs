using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float ExplosionDamage;
    public bool Destroyed = false;

    private bool damaged=false;
    private bool damaged2=false;
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
        if(col.gameObject.layer == 8 && !damaged)
        {
            Debug.Log("enemyBoom");
            col.gameObject.GetComponent<EnemyHealthSystem>().Damage(ExplosionDamage);
            damaged = true; 
        }
        else if(col.gameObject.layer == 3 && !damaged2)
        {
            Debug.Log("playerBoom");
            col.gameObject.GetComponent<HealthSystem>().Damage(ExplosionDamage);
            damaged2 = true;
        }
        
    }
}
