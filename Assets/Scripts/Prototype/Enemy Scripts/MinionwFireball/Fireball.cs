using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed;
    Transform PlayerPosition;
    private Vector2 target;
    public float FireballDamage = 15f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(PlayerPosition.position.x - transform.position.x, PlayerPosition.position.y - transform.position.y);
        Destroy(gameObject, 4f);
        GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(target) * fireballSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void DestroyFireball()
    {
        Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DestroyFireball();
            collision.transform.SendMessage("DamagePlayer", FireballDamage);
        }
        if(collision.CompareTag("Ground"))
        {
            DestroyFireball();
        }
    }
}
