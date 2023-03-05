using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spagetti : MonoBehaviour
{
    public GameObject soul;

    IEnumerator SpawnSoul(float wait, Collider2D col)
    {
        yield return new WaitForSeconds(wait);
        Instantiate(soul, transform.position + new Vector3(0,0.3f,0), Quaternion.identity).GetComponent<SoulMovement>().player = col.transform.parent.parent;
        Instantiate(soul, transform.position, Quaternion.identity).GetComponent<SoulMovement>().player = col.transform.parent.parent;
        Instantiate(soul, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity).GetComponent<SoulMovement>().player = col.transform.parent.parent;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(SpawnSoul(0.8f, collision));
            GetComponent<Animator>().SetBool("Dead", true);
        }
    }
}
