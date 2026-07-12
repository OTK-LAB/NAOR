using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossKey : MonoBehaviour
{
    public Boss_Manager boss;
    public GameObject key;
    // Start is called before the first frame update
    void Start()
    {
        key.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!boss.isAlive() && !key.GetComponent<DestroyedKey>().isAcquired())
        {
            key.SetActive(true);
        }
    }
}
