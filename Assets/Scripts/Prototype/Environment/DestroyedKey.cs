using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedKey : MonoBehaviour
{
    private bool acquired = false;
    private void OnTriggerEnter2D(Collider2D collision)

    {
        if (collision.gameObject.tag == "Player") { 
            ScoreText.coinAmount += 1;
            Debug.Log("Key count:" + ScoreText.coinAmount);
            acquired = true;
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public bool isAcquired()
    {
        return acquired;
    }
}
