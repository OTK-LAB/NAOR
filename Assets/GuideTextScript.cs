using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
}
