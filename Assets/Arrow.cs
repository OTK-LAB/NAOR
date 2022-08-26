using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Arrow : MonoBehaviour
{

    Transform PlayerPosition;
    [SerializeField] private float daggerSpeed;
    private Rigidbody2D rb;
    private Vector2 direction;
    public static float daggerDamage = 5f;
    public static Arrow instance;
    float y, x;
    bool b = false;
    public bool left;
    public bool right;
    public GameObject InteractSystemTrigger;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        rb = this.GetComponent<Rigidbody2D>();
        x = PlayerPosition.position.x + 20;
        if (this.gameObject.tag=="uparrow")
            y = PlayerPosition.position.y + 1;
        else if(this.gameObject.tag=="downarrow")
            y = PlayerPosition.position.y - 0.4f ;

        this.gameObject.transform.position = new Vector2(x, y);

        if (left) 
        { 
            direction = Vector2.left;
            this.gameObject.GetComponent<SpriteRenderer>().flipX=true;
        }
        else if(right)
        {
            direction = Vector2.right;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        Destroy(gameObject, 10f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction*daggerSpeed;
    }
    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractSystemTrigger.GetComponent<InteractSystem>().arrowHit = true;
            Debug.Log("dead");
            Destroy(gameObject);
           // this.gameObject.SetActive (false);
            b = true;
        }

    }
}
