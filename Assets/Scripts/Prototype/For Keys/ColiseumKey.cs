using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiseumKey : MonoBehaviour
{
    private static ColiseumKey _instance;
    public static ColiseumKey instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<ColiseumKey>();
            return _instance;
        }
    }

    public GameObject key;

    public float deadEnemyCount;
    public float requiredDeadEnemy;
    private void Awake() 
    {
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
