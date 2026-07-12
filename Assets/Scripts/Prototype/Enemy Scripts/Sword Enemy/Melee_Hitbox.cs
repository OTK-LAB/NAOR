using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Hitbox : MonoBehaviour
{
    Transform PlayerPosition;
    private Vector2 target;
    public float Damage = 33;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(PlayerPosition.position.x - transform.position.x, PlayerPosition.position.y - transform.position.y);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SendMessage("DamagePlayer", Damage);
        }
    }
}
