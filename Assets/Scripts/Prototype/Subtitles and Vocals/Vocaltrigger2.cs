using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocaltrigger2 : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioObject clipToPlay;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vocals2.instance.Say(clipToPlay);
            Destroy(gameObject);
        }
           
    }
}
