using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayToHell : MonoBehaviour
{
    [SerializeField] private PlayerManager playerm;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerm.lives = 0;
            playerm.Die();
            StartCoroutine(playerm.RespawnPlayer());
        }
    }
}
