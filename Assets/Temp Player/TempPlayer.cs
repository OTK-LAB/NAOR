using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;

    public float runSpeed;
    public float jumpForce;
    private bool jumpPressed;
    private float xAxis;
    private bool isGrounded;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        Debug.Log(Input.GetAxisRaw("Horizontal"));
        xAxis = Input.GetAxisRaw("Horizontal");


        if (xAxis > 0 && !facingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
        else if (xAxis < 0 && facingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }

        if (isGrounded)
        {
            if (xAxis == 0)
                animator.Play("Idle");
            else
                animator.Play("Run");
        }
        else
            animator.Play("Jump");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpPressed = true;
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(xAxis * runSpeed, rb.velocity.y);

        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
