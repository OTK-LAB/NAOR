using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEditor.Rendering;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float speed;
    public Vector3 LaunchOffset;
    public GameObject Explosion;

    private float timer = 2;
    

    private void Awake()
    {
        Vector3 directionVector;
        if (GameObject.Find("Player").transform.localScale.x > 0)
        {
            directionVector = transform.right + Vector3.up * 2 / 3;
        }
        else
        {
            directionVector = -transform.right + Vector3.up * 2 / 3;
            
            //transform.Translate(-2, 0, 0);
            Quaternion target = Quaternion.Euler(0, -180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, 1);

        }

        gameObject.GetComponent<Rigidbody2D>().AddForce(directionVector * speed , ForceMode2D.Impulse);
        transform.Translate(LaunchOffset);

    }
    void Start()
    {
       
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Instantiate(Explosion,gameObject.transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
