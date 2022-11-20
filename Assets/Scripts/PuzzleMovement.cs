using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMovement : MonoBehaviour
{
    public GameObject mainObject;
    public GameObject Ref;

    public float xMovement;
    public float xTime;


    void Start()
    {
        
    }
    public void StartMovement()
    {
        if (mainObject.tag=="ministairs")
        {

        }
    }
    public void componentMovement()
    {
            LeanTween.moveX(mainObject, Ref.transform.position.x + xMovement, xTime);

    }
}
