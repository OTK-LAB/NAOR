using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCheck : MonoBehaviour
{
    public int moneyRequired;
    public int moneyType;
    public GameObject Currency;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (moneyType == 1)
        {
            if (Currency.GetComponent<CurrencyScript>().currency2 < moneyRequired)
                GetComponent<Button>().interactable = false;
            else
                GetComponent<Button>().interactable = true;
        }
        else
        {
            if (Currency.GetComponent<CurrencyScript>().currency1 < moneyRequired)
                GetComponent<Button>().interactable = false;
            else
                GetComponent<Button>().interactable = true;
        }
            
    }
    private void Update()
    {
        if (moneyType == 1)
        {
            if (Currency.GetComponent<CurrencyScript>().currency2 < moneyRequired)
                GetComponent<Button>().interactable = false;
            else
                GetComponent<Button>().interactable = true;
        }
        else
        {
            if (Currency.GetComponent<CurrencyScript>().currency1 < moneyRequired)
                GetComponent<Button>().interactable = false;
            else
                GetComponent<Button>().interactable = true;
        }
    }
}
