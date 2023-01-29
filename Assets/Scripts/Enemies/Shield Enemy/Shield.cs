using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    GameObject parent;
    void Start()
    {
        parent = this.transform.parent.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("wall"))
        {
            parent.GetComponent<ShieldEnemy>().turn();
        }
    }
}
