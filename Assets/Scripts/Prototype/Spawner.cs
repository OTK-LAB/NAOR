
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static bool bossKeyAcquired = false;
    public GameObject enemy;
    public GameObject voice;

    private void OnTriggerEnter2D(Collider2D other) {
        if(!bossKeyAcquired)
        {
            if(other.CompareTag("Player"))
            {            
                enemy.SetActive(true);
                voice.SetActive(true);
            }
        }
    }
}
