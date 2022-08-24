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
        


        LeanTween.moveX(miniStairsForward,-74, 10);
        LeanTween.moveX(miniStairsBack, -71, 10).setDelay(5f);
        //  LeanTween.moveX(miniStairsBack, -74, 10).setDelay(10f);



    }
    
    public void platformController()
    {

        Debug.Log("Platform haraket ediyor");
        LeanTween.moveX(platformForward, -6, 4); //if it hasn't delay second one be active
        LeanTween.moveX(platformBack, 2, 4).setDelay(10f);
    }


}
