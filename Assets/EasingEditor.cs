using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingEditor : MonoBehaviour
{
    public GameObject platformForward;
    public GameObject platformBack;
    public GameObject miniStairsForward;
    public GameObject miniStairsBack;


    void Start()
    {
       


    }
    
    public void platformController()
    {
        Debug.Log("Platform haraket ediyor");
        LeanTween.moveX(platformForward, -6, 4); //if it hasn't delay second one be active
        LeanTween.moveX(platformBack, 2, 4).setDelay(10f);
    }

    public void miniStairsGo()
    {
        Debug.Log("merdivenler gidiyo");
        LeanTween.moveX(miniStairsForward, -74, 10);
       
    }

    public void miniStairsCome()
    {
        Debug.Log("merdivenler geliyor");
        LeanTween.moveX(miniStairsBack, -71, 10);
    }


}
