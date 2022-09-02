using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingEditor : MonoBehaviour
{
    public GameObject platform;
    public GameObject miniStairs;

    
    public void platformController()
    {
        Debug.Log("Platform haraket ediyor");
        LeanTween.moveX(platform, -6, 2); //if it hasn't delay second one be active
        LeanTween.moveX(platform, 2, 1).setDelay(4f);
    }

    public void miniStairsGo()
    {
        Debug.Log("merdivenler gidiyo");
        LeanTween.moveX(miniStairs, -74, 1); //moves 'current position' to -74 in 3 second
       
    }

    public void miniStairsCome()
    {
        Debug.Log("merdivenler geliyor");
        LeanTween.moveX(miniStairs, -71, 1);
    }


}
