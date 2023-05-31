using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FetchObjectScript : MonoBehaviour
{
    public GameObject Button;
    bool isFetched = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.GetComponent<Button>().interactable = true;
            isFetched = true;
            Destroy(this.gameObject);
        }
    }
}
