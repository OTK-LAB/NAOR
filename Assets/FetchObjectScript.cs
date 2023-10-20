using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FetchObjectScript : MonoBehaviour
{
    public GameObject Button;
    public bool isFetched = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.GetComponent<Button>().interactable = true;
            isFetched = true;
            this.gameObject.SetActive(false);
        }
    }
}
