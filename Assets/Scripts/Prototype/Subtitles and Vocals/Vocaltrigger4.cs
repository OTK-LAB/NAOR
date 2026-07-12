using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocaltrigger4: MonoBehaviour
{
    // Start is called before the first frame update
    public AudioObject clipToPlay;
    public static bool isTriggered = false;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isTriggered)
        {
            if (collision.CompareTag("Player"))
            {
                Vocal4.instance.Say(clipToPlay);
                isTriggered = true;
                Destroy(gameObject);
            }
        }
    }
}
