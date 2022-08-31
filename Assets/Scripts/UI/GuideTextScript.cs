using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTextScript : MonoBehaviour
{
    public GameObject guideText;
    // Start is called before the first frame update
    void Start()
    {
        
        guideText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            guideText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            guideText.SetActive(false);
        }
    }
}
