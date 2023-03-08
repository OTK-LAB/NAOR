using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsArrow : MonoBehaviour
{
    private Vector2 target;
    public float ArrowDamage;
    private Rigidbody2D rb;
    private float travelDistance;
    private float xStartPos;
    private bool isGravityOn = false;
    private bool hasItGround = false;
    
    public static bool disabled = false;
    private GameObject player;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;
    [SerializeField]
    private Transform damagePosition;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, 10f);
        rb = GetComponent<Rigidbody2D>();
        rb.position = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            player.GetComponent<HealthSystem>().Damage(ArrowDamage);
        }
        if (collision.gameObject.layer == 6)
        {
            hasItGround = true;
            Destroy(gameObject, 3f);
            rb.velocity = Vector2.zero;
        }
    }
}
