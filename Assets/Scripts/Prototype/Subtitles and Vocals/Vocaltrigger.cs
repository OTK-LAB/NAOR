using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocaltrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioObject clipToPlay;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Vocals.instance.Say(clipToPlay);
        Destroy(gameObject, 1f);
    }
}
