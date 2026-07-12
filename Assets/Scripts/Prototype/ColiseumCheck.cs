using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiseumCheck : MonoBehaviour
{

    public static bool isEntered = false;

    private void Start() {
        if(isEntered)
        {
            gameObject.SetActive(false);
        }   
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            isEntered = true;
        }
    }
}
