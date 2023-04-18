using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{

    private Ultimate2DPlayer player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
       // rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bounce!");
            //collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            //rb.velocity = new Vector2(rb.velocity.x, bounce);
            //collision.gameObject.GetComponent<Rigidbody2D>().MovePosition(collision.gameObject.GetComponent<Rigidbody2D>().position + Vector2.up * bounce);

            //rb.position += new Vector2(0, bounce);
            player = collision.gameObject.GetComponent<Ultimate2DPlayer>();
            player._stateMachine.ChangeState(player.JumpState);
        }
    }
    
}
