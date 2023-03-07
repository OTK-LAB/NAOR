using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;


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
            if (platforms == Platforms.CrushingTrap && !loop)
            {
                loop = true;
                //Crushing Trap *************************************************************************
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(TargetObj.transform.DOLocalMoveY(-2, 0.5f).SetEase(Ease.InBack));
                mySequence.Append(TargetObj.transform.DOLocalMoveY(0, 4).SetEase(Ease.InQuad));
                mySequence.SetLoops(-1, LoopType.Restart);
            }
            if (platforms == Platforms.MovingBridge && interaced)
            {
                TargetObj.transform.DORotate(new Vector3(0, 0, 0), 3f);
            }
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
