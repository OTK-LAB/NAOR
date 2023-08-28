using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
//using Unity.VisualScripting.ReorderableList;
using UnityEditor;


public class PlatformController : MonoBehaviour
{
    [SerializeField]
    private Platforms platforms = Platforms.None;
    [SerializeField]
    public PlayerInputActions inputActions;
    public GameObject TargetObj;
    private bool interaced;
    private bool isInRange = false;
    private bool loop;

    [SerializeField]
    /*
     public float CTDownSpeed;
    public float CTUpSpeed;
    public float MTSpeed;
    */
    public float firstDestination;
    public float firstDuration;
    public float secondDestination;
    public float secondDuration;



    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.PlatformInteraction.started += Interacted;
        inputActions.Interaction.PlatformInteraction.performed += Interacted;
        inputActions.Interaction.PlatformInteraction.canceled += Interacted;
    }
    void Start()
    {

    }

    void Update()
    {
        if (isInRange)
        {

            if (platforms == Platforms.MovingBridge && interaced)
            {
                TargetObj.transform.DORotate(new Vector3(0, 0, 0), 3f);
            }
            if(platforms == Platforms.LockedDoor && interaced) 
            {
                TargetObj.transform.DORotate(new Vector3(0, 90, 0), 4f);
            }


        }
        if (platforms == Platforms.ElongatedPlatform&& !loop)
        {
            loop = true;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(TargetObj.transform.DOScaleY(firstDestination, firstDuration));//3,3
            mySequence.Append(TargetObj.transform.DOScaleY(secondDestination, secondDuration));//1,3
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        if (platforms == Platforms.CrushingTrap && !loop)
        {
            loop = true;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(TargetObj.transform.DOLocalMoveY(firstDestination, firstDuration).SetEase(Ease.InBack));//-1.5f,0.5f
            mySequence.Append(TargetObj.transform.DOLocalMoveY(secondDestination, secondDuration).SetEase(Ease.InQuad));//0,3
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        if (platforms == Platforms.MovingPlatformY && !loop)
        {
            loop = true;

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(TargetObj.transform.DOLocalMoveY(firstDestination, firstDuration));//5,2
            mySequence.Append(TargetObj.transform.DOLocalMoveY(secondDestination, secondDuration));//0,2
            mySequence.SetLoops(-1, LoopType.Restart);
        }
        if (platforms == Platforms.MovingPlatformX && !loop)
        {
            loop = true;

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(TargetObj.transform.DOLocalMoveX(firstDestination, firstDuration).SetEase(Ease.InOutSine));//5,2
            mySequence.Append(TargetObj.transform.DOLocalMoveX(secondDestination, secondDuration).SetEase(Ease.InOutSine));//0,2
            mySequence.SetLoops(-1, LoopType.Restart);
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
            }
        }

        void Interacted(InputAction.CallbackContext context)
        {
            interaced = context.ReadValueAsButton();
        }
    }