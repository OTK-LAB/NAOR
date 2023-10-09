using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEditor.Rendering;
using UnityEngine;

public class SlowBombScript : MonoBehaviour
{
    public float speed;
    public Vector3 LaunchOffset;
    public PlayerInputActions inputActions;
    public bool thrown;
    public int direction;
    public GameObject Slowdown;


    private GameObject player;


    private void Awake()
    {
        Vector3 directionVector;
        if (GameObject.Find("NewPlayer").transform.localScale.x > 0)
        {
            directionVector = transform.right + Vector3.up * 2 / 3;
        }
        else
        {
            directionVector = -transform.right + Vector3.up * 2 / 3;
            transform.Translate(-2, 0, 0);
        }

        gameObject.GetComponent<Rigidbody2D>().AddForce(directionVector * speed, ForceMode2D.Impulse);
        transform.Translate(LaunchOffset);

    }
    void Start()
    {

    }

    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Instantiate(Slowdown, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
