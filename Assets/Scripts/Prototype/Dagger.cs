using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [SerializeField] private float daggerSpeed;
    private Rigidbody2D rb;
    private Vector2 direction;
    public static float daggerDamage = 5f;
    public static Dagger instance;
 
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = direction * daggerSpeed;
    }
    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    public void DaggerDamageUp(){
        daggerDamage += daggerDamage * 1/2;
    }
    public static void DaggerDamageDown(){
        daggerDamage = 5.0f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Minion_wfireball>().TakeDamage(daggerDamage);
            this.gameObject.SetActive(false);
        }
        if(collision.CompareTag("Villager"))
        {
            collision.GetComponent<VillagerHealthManager>().TakeDamage(daggerDamage);
            this.gameObject.SetActive(false);
        }
        
        if(collision.CompareTag("Sword"))
        {
            collision.GetComponent<Sword_Behaviour>().TakeDamage(daggerDamage);
            this.gameObject.SetActive(false);
        }
        if(collision.CompareTag("MinionwPoke"))
        {
            collision.GetComponent<Minion_wpoke>().TakeDamage(daggerDamage);
            this.gameObject.SetActive(false);
        }
        if(collision.CompareTag("Legolas"))
        {
            collision.GetComponent<Legolas>().TakeDamage(daggerDamage);
            this.gameObject.SetActive(false);
        }
    }
}
