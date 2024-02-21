using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShallowGrave : MonoBehaviour
{
    public GameObject Player;
    public GameObject InteractionText;
    public PlayerInputActions inputActions;
    public GameObject ManaStop;
    public GameObject Currency;

    bool active = false;
    bool inRange = false;
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Interaction.Enable();

        inputActions.Interaction.ShallowGraveInteraction.started += OnShallowGraveTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !active)
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

    void OnShallowGraveTriggered(InputAction.CallbackContext context)
    {
        if (inRange)
        {
            Player.GetComponent<ManaSoulSystem>().CurrentMana = Player.GetComponent<ManaSoulSystem>().MaxMana;
            Player.GetComponent<HealthSystem>().CurrentHealth = Player.GetComponent<HealthSystem>().MaxHealth;
            //Player.GetComponent<PlayerController>().lastCheckpointPosition = transform.position;
            active = true;
            this.GetComponent<SpriteRenderer>().color = Color.green;
            ManaStop.GetComponent<ManaStop>().isActive = true;
        }
    }

    public void Respawn()
    {
        Player.GetComponent<ManaSoulSystem>().CurrentMana = Player.GetComponent<ManaSoulSystem>().MaxMana;
        Player.GetComponent<HealthSystem>().CurrentHealth = Player.GetComponent<HealthSystem>().MaxHealth;
        Player.GetComponent<ManaSoulSystem>().currentSoul = 0;
        Currency.GetComponent<CurrencyScript>().currency1 = Currency.GetComponent<CurrencyScript>().currency1 * 3 / 4;
        Currency.GetComponent<CurrencyScript>().currency2 = Currency.GetComponent<CurrencyScript>().currency2 * 3 / 4;
        //Player.transform.position = Player.GetComponent<PlayerController>().lastCheckpointPosition;
    }
}
