using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEditor : MonoBehaviour
{
    public float xTween;
    public float xTime;
    void Start()
    {
        
    }

    public void move(GameObject relatedObject1,GameObject relatedObject2)
    {
        LeanTween.moveX(relatedObject1 , relatedObject2.transform.position.x + xTween, xTime);
        LeanTween.moveY(relatedObject1, relatedObject2.transform.position.x + xTween, xTime);

    }


}
