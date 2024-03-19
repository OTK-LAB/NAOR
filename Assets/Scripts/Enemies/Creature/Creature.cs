using UnityEngine;
using UltimateCC;

public class Creature : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform enemy;
    [SerializeField] private Rigidbody2D rb;
    public float speed;
    public float stopChaseDistance;
    public float dashDistance;
    public float dashTime;
    public float maxDashHeight;
    [BoundedCurve] public AnimationCurve dashHeightCurve;
    [NonEditable] public AnimationCurve dashYVelocityCurve;
    [BoundedCurve] public AnimationCurve dashXVelocityCurve;
    [NonEditable] public float curveTime;

    private enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack1,
        Attack2,
        Die
    }
    [SerializeField] private State state;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        animator.SetBool(state.ToString(), true);
        rb = GetComponent<Rigidbody2D>();
        dashYVelocityCurve = dashHeightCurve.Derivative();
    }

    private void Update()
    {
        if (enemy)
        {
            rb.velocity = ApplyMovement();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        if (enemy && Vector2.Distance(transform.position, enemy.position) > stopChaseDistance)
        {
            enemy = null;
        }
        curveTime += Time.deltaTime;
    }

    private Vector2 ApplyMovement()
    {
        Vector2 velocity = Vector2.zero;
        if (Vector2.Distance(transform.position, enemy.position) > dashDistance)
        {
            velocity = new(Mathf.Sign(enemy.position.x - transform.position.x) * speed, rb.velocity.y);
            curveTime = 0f;
        }
        else if (curveTime / dashTime >= 1f)
        {
            ChangeState(State.Idle);
            velocity = new(0, rb.velocity.y);
        }
        else
        {
            velocity.x = Mathf.Sign(enemy.position.x - transform.position.x) * dashXVelocityCurve.Evaluate(curveTime / dashTime) * (dashDistance - 1f) / dashTime;
            velocity.y = dashYVelocityCurve.Evaluate(curveTime / dashTime) * maxDashHeight / dashTime;
        }

        if (Mathf.Sign(velocity.x) != Mathf.Sign(transform.GetChild(0).localScale.x) && velocity.x != 0f)
        {
            var scale = transform.GetChild(0).localScale;
            scale.x *= -1;
            transform.GetChild(0).localScale = scale;
        }
        return velocity;
    }

    // make overlapcircle ?? maybe, if this is not consistant
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (!enemy || Vector2.Distance(transform.position, enemy.position) > Vector2.Distance(transform.position, collision.transform.position))
            {
                enemy = collision.transform;
                ChangeState(State.Run);
            }
        }
    }

    private void ChangeState(State newState)
    {
        animator.SetBool(state.ToString(), false);
        state = newState;
        animator.SetBool(newState.ToString(), true);
    }
}
