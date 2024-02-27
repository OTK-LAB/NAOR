using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class ArchersArrow : MonoBehaviour
{
    private Vector3 fire_loc;
    private Vector2 target;
    Transform PlayerPosition;
    public float ArrowDamage;
    private Rigidbody2D rb;
    private float travelDistance;
    private float xStartPos;
    public float arrowSpeed;
    private bool isGravityOn = false;
    private bool hasItGround = false;
    public GameObject fire;
    public GameObject archer;
    private GameObject player;
    public static bool disabled = false;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;

    private HealthSystem playerHealthSystem;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerPosition = player.transform;
        playerHealthSystem = PlayerMain.Instance.PlayerData.healthSystem;
        target = new Vector2(PlayerPosition.position.x - transform.position.x, PlayerPosition.position.y - transform.position.y);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(target * archer.GetComponent<Archer>().LaunchForce);
        rb.velocity = Vector3.Normalize(target) * arrowSpeed;
        rb.rotation = 0;
        xStartPos = transform.position.x;
        Destroy(gameObject, 10f);
    }
    void Update()
    {
        if (!hasItGround)
        {
            rb.position = transform.position;
            if (isGravityOn)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            playerHealthSystem.Damage(ArrowDamage);
            //player.GetComponent<HealthSystem>().Damage(ArrowDamage);
        }
        if (collision.gameObject.layer == 6)
        {
            fire_loc = new Vector3(transform.position.x, (transform.position.y + 1f), 0);
            Instantiate(fire, fire_loc, Quaternion.LookRotation(Vector3.forward, fire_loc));
            hasItGround = true;
            rb.gravityScale = 0.0f;
            rb.simulated = false;
            Destroy(gameObject, 3f);
            rb.velocity = Vector2.zero;
        }
    }
    public void Fire(float speed, float travelDistance, float damage)
    {
        arrowSpeed = speed;
        this.travelDistance = travelDistance;
        ArrowDamage = damage;
    }
}
