using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EasingEditor : MonoBehaviour
{
    public GameObject platform;
    public GameObject miniStairs;
    public GameObject getaway;
    private Transform stairTriggerTransform;
    private Transform buttonTriggerTransform;
    private Transform getawayTriggerTransform;
    public float stairsMovement;

    public void Start()
    {

    }
    public void platformController()
    {
        buttonTriggerTransform = GameObject.FindGameObjectWithTag("platform").GetComponent<Transform>();
        Debug.Log("Platform haraket ediyor");
        LeanTween.moveX(platform, (buttonTriggerTransform.position.x - 9) - 3, 2); //if it hasn't delay second one be active
        LeanTween.moveX(platform, buttonTriggerTransform.position.x - 9, 1).setDelay(4f);
    }

    public void miniStairsGo()
    {
        stairTriggerTransform = GameObject.FindGameObjectWithTag("stairs").GetComponent<Transform>();
        Debug.Log("merdivenler gidiyo");
        LeanTween.moveX(miniStairs, stairTriggerTransform.position.x + 15.68f - 2.62f, 1); //moves 'current position' to -74 in 3 second
                                                                                           //-2,62f
    }

    public void miniStairsCome()
    {
        stairTriggerTransform = GameObject.FindGameObjectWithTag("stairs").GetComponent<Transform>();
        Debug.Log("merdivenler geliyor");
        LeanTween.moveX(miniStairs, stairTriggerTransform.position.x + 15.68f, 1);
    }

    public void getawayController()
    {
        getawayTriggerTransform = GameObject.FindGameObjectWithTag("getaway").GetComponent<Transform>();
        LeanTween.moveX(getaway, getawayTriggerTransform.position.x - 8, 1);
        Debug.Log("geçit kapanýyor");

    }


}
