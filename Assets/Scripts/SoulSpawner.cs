using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoulSpawner : MonoBehaviour
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
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = this.player;
        StartCoroutine(spawnSoul(3f));
    }
}
