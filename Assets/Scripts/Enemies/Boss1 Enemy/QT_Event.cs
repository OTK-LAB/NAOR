using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QT_Event : MonoBehaviour
{
    public Boss1Manager boss;
    private Image image;

    public float pressAmount = 0;
    public float timeThreashold;
    
    void Start()
    {
        image = GetComponent<Image>();
        pressAmount = 0;
    }


    void Update()
    {
        image.fillAmount = pressAmount;
        timeThreashold += Time.deltaTime;

        if (pressAmount >= 1)
        {
            //event success
        }
    }

    public void PlayerPressed()
    {
        if (pressAmount < 1f)
        {
            Debug.Log("PRESSTO");
            pressAmount += 0.0144f;
        }

    }
}
