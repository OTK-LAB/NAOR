using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Arise : MonoBehaviour
{
    PlayerInputActions _playerInputActions;
    PlayerStateMachine _context;
    Animator _animator;
    bool rose = false;
    // Start is called before the first frame update
    void Awake()
    {
        _context = GetComponent<PlayerStateMachine>();
        _context.enabled = false;
        _animator = GetComponent<Animator>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Move.started += OnMovementInput;
        _playerInputActions.Player.Move.canceled += OnMovementInput;
        _playerInputActions.Player.Move.performed += OnMovementInput;
    }

    // Update is called once per frame
    void Update()
    {
        if(!rose)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        }
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        if(!rose)
        {
            StartCoroutine(wait());
            rose = true;
        }
    }
    IEnumerator wait()
    {
        _animator.Play("PlayerArise");
        yield return new WaitForSeconds(1.14f);
        _context.enabled = true;
    }
}
