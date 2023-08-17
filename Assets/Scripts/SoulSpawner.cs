using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoulSpawner : MonoBehaviour
{
    public GameObject soul;
    public Transform player;
    public Transform parent;
    private bool toggle = true;
    private void Start()
    {
        parent = transform.parent;
    }

    private void FixedUpdate()
    {
        if (parent.GetComponent<HealthSystem>().currentHealth == 0f && toggle)
        {
            //StartCoroutine(spawnSoul(0.8f));
            toggle = false;
        }
    }

    private IEnumerator spawnSoul(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = this.player;
    }
}
