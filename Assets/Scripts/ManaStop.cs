using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManaStop : MonoBehaviour
{
    public GameObject Player;
    public GameObject InteractionText;
    public PlayerInputActions inputActions;

    public bool isActive = true;
    bool inRange = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.ManaStopInteraction.started += OnManaStopTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && isActive)
        {
            inRange = true;
            InteractionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            inRange = false;
            InteractionText.SetActive(false);
            inputActions.Interaction.Enable();
        }
    }

    public void RecoverMana()
    {
        Player.GetComponent<ManaSoulSystem>().AddMana(Player.GetComponent<ManaSoulSystem>().MaxMana / 2);
        inputActions.Interaction.Disable();
        isActive = false;
    }

    void OnManaStopTriggered(InputAction.CallbackContext context)
    {
        if (inRange)
        {
            RecoverMana();
        }
    }
}
