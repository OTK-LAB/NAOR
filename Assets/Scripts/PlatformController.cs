using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlatformController : MonoBehaviour
{
    public GameObject TargetObj;
    private bool isInRange = false;
    private bool loop;

    void Start()
    {
        
    }

    void Update()
    {
        Sequence mySequence = DOTween.Sequence();
        if (isInRange && !loop)
        {
            loop= true;
            mySequence.Append(TargetObj.transform.DOLocalMoveY(-2, 0.5f).SetEase(Ease.InBack));
            mySequence.Append(TargetObj.transform.DOLocalMoveY(0, 4).SetEase(Ease.InQuad));
            mySequence.SetLoops(100, LoopType.Restart);

        }
        

    }





    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isInRange = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isInRange = false;
            loop= false;
        }
    }
}
