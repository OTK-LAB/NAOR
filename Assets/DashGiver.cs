using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;

public class DashGiver : MonoBehaviour
{
    public PlayerMain player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            player.PlayerData.Dash.CanDash = true;
            Destroy(gameObject);
        }
    }
    
}
