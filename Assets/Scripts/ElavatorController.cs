using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElavatorController : MonoBehaviour
{
    public PlayerInputActions inputActions;
    public Transform UpTrigger;
    public Transform DownTrigger;
    public GameObject Platform;
    public float time;

    private bool canTrigger=true;
    private bool direction=true;// yukarý 1 aþaðý 0
    bool inRange = false;
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.ElevatorInteraction.started += OnElevatorPushed;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
   
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            inRange = false;
            inputActions.Interaction.Enable();
        }
    }

    void OnElevatorPushed(InputAction.CallbackContext context)
    {
        if (inRange&& canTrigger)
        {
            if(direction)
            {
                LeanTween.moveY(Platform, UpTrigger.position.y, time).setEaseInCubic();
                StartCoroutine(CanTrigger(time));
            }
            else if(!direction)
            {
                LeanTween.moveY(Platform, DownTrigger.position.y, time).setEaseInCubic();
                StartCoroutine(CanTrigger(time));
            }
        }
    }
    IEnumerator CanTrigger(float time)
    {
        direction = !direction;
        canTrigger = false;
        yield return new WaitForSeconds(time);
        canTrigger = true;

    }
}
