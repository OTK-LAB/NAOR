using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class BossThrowableObject : MonoBehaviour
{
    [HideInInspector] public PlayerMain playerMain;
    Rigidbody2D rigid;


    public float damage;
    public float setCount;
    public float throwSpeed;



    private float count;
    private bool readytogo;
    private bool curveThrow;
    private bool normalThrow;
    Vector2 controlPointThrow;
    Vector2 directionPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        readytogo = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (readytogo)
        {
            if (count < setCount && curveThrow)
            {
                count += Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(transform.position, controlPointThrow, count);
                Vector3 m2 = Vector3.Lerp(controlPointThrow, directionPlayer, count);
                rigid.MovePosition(Vector3.Lerp(m1, m2, count));                                                            //slow
            }

            if (normalThrow)
            {
                rigid.MovePosition((Vector2)transform.position + (directionPlayer * throwSpeed * Time.deltaTime));          //fast
            }



        }



    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (readytogo)
        {
            if (collision.tag == "Player")
            {
                PlayerMain.Instance.PlayerData.healthSystem.Damage(damage);
                Debug.Log(damage);
                Hit();
            }
            else if (collision.tag == "wall" || collision.tag == "ground")
            {
                Hit();
            }
        }
    }

    public void ThrowItem(GameObject toWhere)
    {
        int random = Random.Range(1, 10);

        if (random < 6)
        {

            count = 0f;
            controlPointThrow = ((transform.position + toWhere.transform.position) / 2f) + new Vector3(0f, 10f, 0f);
            directionPlayer = new Vector2(toWhere.transform.position.x, toWhere.transform.position.y - 5f);

            curveThrow = true;
        }
        else
        {
            directionPlayer = new Vector3(toWhere.transform.position.x, toWhere.transform.position.y, toWhere.transform.position.z) - transform.position;


            normalThrow = true;
        }

        readytogo = true;
    }

    void Hit()
    {
        //visual effects go here
        Destroy(gameObject);
    }
}
