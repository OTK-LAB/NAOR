using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEditor.Rendering;
using UnityEngine;

public class DaggerScript : MonoBehaviour
{
    public float speed;
    public Vector3 LaunchOffset;
    public GameObject Explosion;




    private void Awake()
    {
        Vector3 directionVector;
        if (GameObject.Find("NewPlayer").transform.localScale.x > 0)
        {
            directionVector = transform.right ;
        }
        else
        {
            directionVector = -transform.right ;
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
        Instantiate(Explosion, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
