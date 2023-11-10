using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class EffectiveBombScript : MonoBehaviour
{
    public GameObject Player;
    public float speed;
    public Vector3 LaunchOffset;
    public GameObject effectObj;
    private float timer = 2;

    private void Awake()
    {
        ignoreLayers();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Explosive"), LayerMask.NameToLayer("Ground"),false);

        Vector3 directionVector;
        if (GameObject.Find("Player").transform.localScale.x > 0)
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
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ignoreLayers()
    {
        for (int i = 0; i < 32; i++)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Explosive"), i);
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Instantiate(effectObj, new Vector3(gameObject.transform.position.x,col.collider.bounds.max.y,gameObject.transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
