using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingTrap : MonoBehaviour
{

    public GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    public float damage = 10f;
    public float arrowInterval = 2f;

    void Start()
    {
        InvokeRepeating("ShootArrow", arrowInterval, arrowInterval);
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = transform.forward * arrowSpeed;
    }

}
