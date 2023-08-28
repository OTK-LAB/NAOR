using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;

public class PitBehaviour : MonoBehaviour
{
    public PlayerMain player;
    public Transform position;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            player.transform.position = position.position;
        }
    }
}
