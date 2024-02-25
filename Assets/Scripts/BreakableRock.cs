using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    public GameObject SpawnObject;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            OnHit();
        }
    }
    void OnHit()
    {
        if(SpawnObject != null)
        {
            Instantiate(SpawnObject,new Vector3(transform.position.x, transform.position.y,0), Quaternion.identity);
        }
        Destroy(this.gameObject);
        
    }
}
