using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    [SerializeField]
    private float bounce = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
}
