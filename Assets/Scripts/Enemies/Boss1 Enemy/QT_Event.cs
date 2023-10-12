using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QT_Event : MonoBehaviour
{
    public Boss1Manager boss;
    private Image image;

    private float pressAmount = 0;
    public float settimeThreashold;
    private float timeThreashold;


    void Start()
    {
        image = GetComponent<Image>();
        pressAmount = 0;
        timeThreashold = settimeThreashold;
    }


    void Update()
    {
        if (gameObject.activeSelf)
        {
            image.fillAmount = pressAmount;

            if (timeThreashold > 0)
                timeThreashold -= Time.deltaTime;
            else
            //player looses but idk how to do this mike

            if (pressAmount >= 1)
            {
                boss.notDead = false;
            }
        }

    }

    public void PlayerPressed()
    {
        if (pressAmount < 1f && timeThreashold > 0)
        {
            pressAmount += 0.0144f;
        }

    }
}
