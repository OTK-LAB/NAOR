using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public GameObject player;
    public HealthSystem HealthSystem;
    public ManaSoulSystem ManaSoulSystem;
    public PlayerInputActions inputActions;
    public InventoryScriptable playerInventory;
    public InventoryScriptable shopInventory;
    public Inventory PlayerInventoryManager;
    public Inventory ShopInventoryManager;
    public Ultimate2DPlayer playerData;


    public bool consumableInteracted;
    public Item selectedItem;
    public Item shopSelectedItem;
    

    private bool indicator;
    public delegate void myFunciton();
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Consumable.started += OnConsumableTriggered;

        shopSelectedItem = playerInventory.nullitem;
        selectedItem = playerInventory.nullitem;
    }
    private void Start()
    {
    }
    IEnumerator DelayCoroutine(Item _selectedItem)
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSeconds(_selectedItem.time);
        Debug.Log("Coroutine resumed after time seconds");
        if (_selectedItem.id == 2)
        {
            HealthSystem.broccoli = false;
        }
        else if(_selectedItem.id == 4)
        {
            playerData.PlayerData.Consume.HorizontalSpeedMultiplier = 1;
        }
        else if(_selectedItem.id == 5)
        {
            playerData.PlayerData.Consume.AttackMultiplier= 1;
        }
    }
    private void Update()
    {
        shopSelectedItem = ShopInventoryManager.GetSelectedItem();
        selectedItem = PlayerInventoryManager.GetSelectedItem();
        if (consumableInteracted && selectedItem.isEquiped)
        {
            Consume();
        }
    }
    public void Consume()
    {
        consumableInteracted = false;

        switch (selectedItem.id)
        {
            case 1:
                Apple(selectedItem);
                break;
            case 2:
                Broccoli(selectedItem);
                break;
            case 3:
                SpringWater(selectedItem);
                break;
            case 4:
                EnergyDrink(selectedItem);
                break;
            case 5:
                Spinach(selectedItem);
                break;
            default:
                break;
        }
        playerInventory.RemoveItem(selectedItem);

    }
    public void Apple(Item _selectedItem)
    {
        HealthSystem.Heal(_selectedItem.value);
    }
    public void Broccoli(Item _selectedItem)
    {
        HealthSystem.broccoli=true;
        StartCoroutine(DelayCoroutine(_selectedItem));
    }
    public void SpringWater(Item _selectedItem)
    {
        ManaSoulSystem.AddMana(33);
    }
    public void EnergyDrink(Item _selectedItem)
    {
        playerData.PlayerData.Consume.HorizontalSpeedMultiplier = 1.25f;
        StartCoroutine(DelayCoroutine(_selectedItem));
    }
    public void Spinach(Item _selectedItem)
    {
        playerData.PlayerData.Consume.AttackMultiplier = 1.25f;
        StartCoroutine(DelayCoroutine(_selectedItem));

    }
    public void Buy()
    {
        playerInventory.AddItem(shopSelectedItem);
    }
    private void OnConsumableTriggered(InputAction.CallbackContext context)
    {
        consumableInteracted = context.ReadValueAsButton();
    }
}

    