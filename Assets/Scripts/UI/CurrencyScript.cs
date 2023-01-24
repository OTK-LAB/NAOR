using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CurrencyScript : MonoBehaviour
{
    public int currency1;
    public int currency2;

    public TextMeshProUGUI currency1Text;
    public TextMeshProUGUI currency2Text;

    private void Start()
    {
        currency1Text.text = currency1.ToString();
        currency2Text.text = currency2.ToString();
    }

    private void Update()
    {
        currency1Text.text = currency1.ToString();
        currency2Text.text = currency2.ToString();
    }

    public void SpendMoney(int amount)
    {
        if (amount > currency2 + currency1 * 10)
            return ;
        else if (amount <= currency2)
            currency2 -= amount;

        else if (amount > currency2)
        {
            amount -= currency2;
            currency2 = 0;
            currency1 -= amount / 10;
            if (amount % 10 != 0)
            {
                ConvertCurrecny1toCurrency2(1);
                currency2 -= amount % 10;
            }
        }
    }

    public void EarnMoney(int amount)
    {
        currency2 += amount;
    }

    public void ConvertCurrecny1toCurrency2(int amount)
    {
        if (currency1 >= amount)
        {
            currency2 += 10 * amount;
            currency1 -= amount;
        }
    }

    public void ConvertCurrecny2toCurrency1(int amount)
    {
        if (currency2 >= amount)
        {
            currency2 -= (amount - amount % 10);
            currency1 += amount / 10;
        }
    }
    
}
