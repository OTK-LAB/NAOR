using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swingtest : MonoBehaviour
{
    public float jumpForce;
    public bool jumpTrue = false;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(jumpTrue)
        {
            GetComponent<HingeJoint2D>().enabled = false;
            rb.velocity = new Vector2((rb.rotation / 4) + rb.velocity.x, jumpForce);
            jumpTrue = false;
        }
    }
}
