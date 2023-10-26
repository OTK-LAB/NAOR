using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;



public class Throwables : MonoBehaviour
{
    public GameObject bomb;
    public PlayerInputActions inputActions;

    private bool _isPressed;
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Throwable.started += OnThrowableStarted;
    }

    void Update()
    {
        
    }

    void OnThrowableStarted(InputAction.CallbackContext context)
    {
        _isPressed = true;
    }

}
