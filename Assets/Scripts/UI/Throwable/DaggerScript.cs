using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEditor.Rendering;
using UnityEngine;

public class DaggerScript : MonoBehaviour
{
    public Item item;
    public float speed;
    public bool iceDagger;

    public GameObject ice;

    public float deathTime;
    public float daggerDamage;
    private bool Destroyed = false;

    public bool isIceBreaked;

    private void Awake()
    {
        daggerDamage = item.value;
        
        Vector3 directionVector;
        if (GameObject.Find("Player").transform.localScale.x > 0)
        {
            directionVector = transform.right ;
            transform.Translate(2, 0, 0);

            Quaternion target = Quaternion.Euler(0,0,-90);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, 1);
        }
        else
        {
            directionVector = -transform.right ;
            transform.Translate(-2, 0, 0);

            Quaternion target = Quaternion.Euler(0, 0, 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, 1);

        }

        gameObject.GetComponent<Rigidbody2D>().AddForce(directionVector * speed, ForceMode2D.Impulse);

    }
    void Start()
    {

    }

    void Update()
    {
        deathTime -= Time.deltaTime;
        if (deathTime <= 0)
        {
            Destroyed= true;
        }
        if (Destroyed)
        {
            Destroy(gameObject);
        }
        
    
    }

    public void breakFreeze()
    {
        Instantiate(ice);
        Destroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (iceDagger)
            {
                col.GetComponent<EnemyController>().frozenState(col);
                gameObject.GetComponent<Collider2D>().enabled =false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                col.gameObject.GetComponent<EnemyHealthSystem>().Damage(daggerDamage);
                Destroyed = true;
            }
        }
    }


}
