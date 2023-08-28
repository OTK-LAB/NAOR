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
        if(other.tag == "Player")
        {
            player.PlayerData.Dash.CanDash = true;
            Time.timeScale = 0;
            dashUI.SetActive(true);
            //WAIT 3 SECONDS
            dashUI.SetActive(false);
            Time.timeScale = 1;
            Destroy(gameObject);
        }
    }
}
