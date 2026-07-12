using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiseumKey : MonoBehaviour
{
    public GameObject key;

    public float deadEnemyCount;
    public float requiredDeadEnemy;

    public static ColiseumKey instance;
    
    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        key.SetActive(false);
        deadEnemyCount = 0;
    }
    private void Update() {
        if(deadEnemyCount == requiredDeadEnemy && !key.GetComponent<DestroyedKey>().isAcquired())
        {
            key.SetActive(true);
        }
    }
}
