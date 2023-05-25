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

    public bool SpendMoney(int amount, int type)
    {
        if (type == 1 && amount <= currency1)
        {
            currency1 -= amount;
            return true;
        }

        else if (type == 2 && amount <= currency2)
        {
            currency2 -= amount;
            return true;
        }
        return false;
    }

    public void EarnMoney(int amount, int type)
    {
        if (type == 1)
            currency1 += amount;
        else
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

    // Buttonlarla kolayca test etmek için kullanýlacak olan fonksiyonlar onunn dýþýnda kullanmayýz
    public void SpendCurrency1(int amount)
    {
        SpendMoney(amount, 1);
    }
    public void SpendCurrency2(int amount)
    {
        SpendMoney(amount, 2);
    }
    public void EarnCurrency1(int amount)
    {
        EarnMoney(amount, 1);
    }
    public void EarnCurrency2(int amount)
    {
        EarnMoney(amount, 2);
    }
}