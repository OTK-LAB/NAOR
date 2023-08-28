using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorController : MonoBehaviour
{
    public PlayerInputActions inputActions;
    public Transform UpTrigger;
    public Transform DownTrigger;
    public GameObject Platform;
    public GameObject Barrier;
    public float time;

    private bool canTrigger=true;
    private bool direction=true;// yukarý 1 aþaðý 0
    bool inRange = false;
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.PlatformInteraction.started += Interacted;
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (inRange && canTrigger)
        {
            if (direction)
            {
                LeanTween.moveY(Platform, UpTrigger.position.y, time);
                StartCoroutine(CanTrigger(time));
            }
            else if (!direction)
            {
                LeanTween.moveY(Platform, DownTrigger.position.y, time);
                StartCoroutine(CanTrigger(time));
            }
            Barrier.gameObject.SetActive(true);
        }
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

    void Interacted(InputAction.CallbackContext context)
    {
      
    }
    IEnumerator CanTrigger(float time)
    {
        direction = !direction;
        canTrigger = false;
        yield return new WaitForSeconds(time);
        canTrigger = true;

    }
}
