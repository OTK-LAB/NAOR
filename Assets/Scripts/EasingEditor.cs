using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EasingEditor : MonoBehaviour
{
    public GameObject platform;
    public GameObject miniStairs;
    private Transform stairTriggerTransform;
    private Transform buttonTriggerTransform;

    public void Start()
    {

    }
    public void platformController()
    {
        buttonTriggerTransform = GameObject.FindGameObjectWithTag("platform").GetComponent<Transform>();
        Debug.Log("Platform haraket ediyor");
        LeanTween.moveX(platform, buttonTriggerTransform.position.x -16, 2); //if it hasn't delay second one be active
        LeanTween.moveX(platform, buttonTriggerTransform.position.x -12 , 1).setDelay(4f);
    }

    public void miniStairsGo()
    {
        stairTriggerTransform = GameObject.FindGameObjectWithTag("stairs").GetComponent<Transform>();
        Debug.Log("merdivenler gidiyo");
        LeanTween.moveX(miniStairs, stairTriggerTransform.position.x +15, 1); //moves 'current position' to -74 in 3 second
    }

    public void miniStairsCome()
    {
        stairTriggerTransform = GameObject.FindGameObjectWithTag("stairs").GetComponent<Transform>();
        Debug.Log("merdivenler geliyor");
        LeanTween.moveX(miniStairs, stairTriggerTransform.position.x + 18, 1);
    }


}
