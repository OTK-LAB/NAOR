using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestThorwable : MonoBehaviour
{
    public float speed;
    public GameObject Throwable;
    public PlayerInputActions inputActions;
    public int direction;
    private bool _triggered;

    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Throwable.started += OnThrowableTriggered;
    }
    void Update()
    {
        if (_triggered)
        {
            Instantiate(Throwable,new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z),Quaternion.identity);
            _triggered= false;
        }
    }

    private void OnThrowableTriggered(InputAction.CallbackContext context)
    {
        _triggered = true;
    }
}
