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
        LeanTween.moveX(platformForward, -6, 2); //if it hasn't delay second one be active
        LeanTween.moveX(platformBack, 2, 1).setDelay(4f);
    }

    public void miniStairsGo()
    {
        Debug.Log("merdivenler gidiyo");
        LeanTween.moveX(miniStairsForward, -74, 3); //moves 'current position' to -74 in 3 second
       
    }

    public void miniStairsCome()
    {
        Debug.Log("merdivenler geliyor");
        LeanTween.moveX(miniStairsBack, -71, 5);
    }


}
