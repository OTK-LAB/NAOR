using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collesium : MonoBehaviour
{
    public static bool keyAcquired = false;
    public GameObject key;
    public Camera cam;

    private List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            enemies.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(!keyAcquired)
        {
            if(other.CompareTag("Player"))
            {      
                foreach(GameObject e in enemies)
                {
                    e.SetActive(true);
                }

            }
        }
    }
}
