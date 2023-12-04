using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;

public class iceEffect : MonoBehaviour
{
    public bool Destroyed = false;
    public Item item;

    private bool damaged = false;
    private bool damaged2 = false;
    private float explosionDamage;

    private void Awake()
    {
        explosionDamage = item.value;
    }
    void Start()
    {

    }
    void Update()
    {
        if (Destroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy") && !damaged)
        {
            col.gameObject.GetComponent<EnemyHealthSystem>().Damage(explosionDamage);
            damaged = true;
        }

    }
}
