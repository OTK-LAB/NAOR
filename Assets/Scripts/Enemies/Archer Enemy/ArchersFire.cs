using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchersFire : MonoBehaviour
{
    public float damage;
    public float fire_time;
    public GameObject player;
    private Animator fire_anim;
    private Rigidbody2D rb;
    private string currentState;
    private float timer;
    private bool stay = false;
    private Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        player = player = GameObject.FindGameObjectWithTag("Player");
        fire_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.rotation = 0;
        Destroy(gameObject, 3f);
       // ChangeAnimationState("fire");
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (stay && timer > (fire_time/10))
        {
            player.GetComponent<HealthSystem>()?.Damage(damage);
            timer = 0;
        }
    }
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        fire_anim.Play(newState);
        currentState = newState;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stay = true;
            coll = collision;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            stay = false;
    }
}
