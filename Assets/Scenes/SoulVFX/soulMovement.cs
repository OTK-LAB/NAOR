using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class soulMovement : MonoBehaviour
{
    public Transform player;
    public Vector2 startVelocity;
    public float speed;
    private Rigidbody2D rb;
    public LayerMask CollectableLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startVelocity.x = Random.Range(-2f, 2f);
        startVelocity.y = Random.Range(-5f, 5f);
    }

    private void Update()
    {
        if (Physics2D.OverlapCircle(player.position + new Vector3(0, -1, 0), 0.1f, CollectableLayer))
        {
            Destroy(this.gameObject, 0.5f);
        }
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position + new Vector3(0, -1, 0), speed * Time.deltaTime);
        rb.velocity = startVelocity;
    }
}
