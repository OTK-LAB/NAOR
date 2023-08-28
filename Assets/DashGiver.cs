using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;

public class DashGiver : MonoBehaviour
{
    public PlayerMain player;
    public GameObject dashUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(DashGive());
        }
    }

    IEnumerator DashGive()
    {
        dashUI.SetActive(true);
        Debug.Log("Before");
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1f;
        Debug.Log("After");
        dashUI.SetActive(false);
        Destroy(gameObject);
    }
}