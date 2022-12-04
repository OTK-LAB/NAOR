using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemyDeath : MonoBehaviour
{
    public GameObject soul;
    public Transform player;
    private void Start()
    {
        StartCoroutine(spawnSoul(3f));
    }

    private IEnumerator spawnSoul(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<soulMovement>().player = this.player;
        StartCoroutine(spawnSoul(3f));
    }
}
