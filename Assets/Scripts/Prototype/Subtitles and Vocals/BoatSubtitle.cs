using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoatSubtitle : MonoBehaviour
{
    public TextMeshProUGUI textbox;
    bool entrance = true;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BoatEnter"))
        {
            //StartCoroutine(thesequence());
            textbox.GetComponent<TextMeshProUGUI>().text = "Press E to ride the boat.";
        }
        if (collision.gameObject.CompareTag("BoatExit"))
        {
            if (entrance)
            {
                StartCoroutine(thesequence());
            }
            
        }
    }
    IEnumerator thesequence()
    {
        yield return new WaitForSeconds(2);
        textbox.GetComponent<TextMeshProUGUI>().text = "Press E to exit the boat.";

    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BoatEnter"))
        {
            //StartCoroutine(thesequence());
            textbox.GetComponent<TextMeshProUGUI>().text = "";
        }
        if (collision.gameObject.CompareTag("BoatExit"))
        {
            //StartCoroutine(thesequence());
            textbox.GetComponent<TextMeshProUGUI>().text = "";
            entrance = false;
        }
    }
}
