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
    public CurrencyScript Currency;

    public bool consumableInteracted;
    public Item selectedItem;
    public Item shopSelectedItem;

    public delegate void myFunciton();
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Consumable.started += OnConsumableTriggered;
        inputActions.UI.Consumable.performed += OnConsumableTriggered;
        inputActions.UI.Consumable.canceled += OnConsumableTriggered;

        shopSelectedItem = playerInventory.nullitem;
        selectedItem = playerInventory.nullitem;
    }
    private void Start()
    {
    }
    IEnumerator EffectCoroutine(Item _selectedItem)
    {
        Debug.Log("Effect Coroutine started");
        yield return new WaitForSeconds(_selectedItem.effectTime);
        Debug.Log("Effect Coroutine resumed after" + _selectedItem.effectTime +" seconds");
        if (_selectedItem.id == 2)
        {
            HealthSystem.broccoli = false;
        }
        else if(_selectedItem.id == 4)
        {
            playerData.PlayerData.Shop.HorizontalSpeedMultiplier = 1;
        }
        else if(_selectedItem.id == 5)
        {
            playerData.PlayerData.Shop.AttackMultiplier= 1;
        }
    }
    IEnumerator DelayCoroutine(Item _selectedItem)
    {
        _selectedItem.inDelay= true;
        Debug.Log("Delay Coroutine started");
        yield return new WaitForSeconds(_selectedItem.delayTime);
        Debug.Log("Delay Coroutine resumed after" + _selectedItem.delayTime + "seconds");
        _selectedItem.inDelay = false;
    }
    private void Update()
    {
        shopSelectedItem = ShopInventoryManager.GetSelectedItem();
        selectedItem = PlayerInventoryManager.GetSelectedItem();
        if (consumableInteracted && selectedItem.isEquiped && !selectedItem.inDelay)
        {
            Consume(selectedItem);
        }
        if (shopSelectedItem.type == "permanent" && shopSelectedItem.stack==1)
        {
            Consume(shopSelectedItem);
            shopSelectedItem.stack = 2;
        }
        consumableInteracted = false;
    }
    
    public void Consume(Item selectedItem)
    {

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
            case 6:
                HpBoost(selectedItem);
                break;
            case 7:
                ManaBoost(selectedItem);
                break;
            case 8:
                AbilityPowerBoost(selectedItem);
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
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
    }
    public void SpringWater(Item _selectedItem)
    {
        ManaSoulSystem.AddMana(33);
    }
    public void EnergyDrink(Item _selectedItem)
    {
        playerData.PlayerData.Shop.HorizontalSpeedMultiplier = _selectedItem.value;
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
    }
    public void Spinach(Item _selectedItem)
    {
        playerData.PlayerData.Shop.AttackMultiplier =_selectedItem.value;
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
    }
    public void HpBoost(Item _selectedItem)
    {
        HealthSystem.maxHealth+=_selectedItem.value;
        HealthSystem.healthBar.SetMaxValue(HealthSystem.maxHealth);
    }
    public void ManaBoost(Item _selectedItem)
    {
        ManaSoulSystem.maxMana+=_selectedItem.value;
        ManaSoulSystem.manaBar.SetMaxValue(ManaSoulSystem.maxMana);

    }
    public void AbilityPowerBoost(Item _selectedItem)
    {
        playerData.PlayerData.Shop.AbilityPowerMultiplier= _selectedItem.value;
    }
    public void Buy() 
    {
        if (playerInventory.CanBuy(shopSelectedItem))
        {
            if (Currency.SpendMoney(shopSelectedItem.price, 2))
            {
                    playerInventory.AddItem(shopSelectedItem);
            }
        }
    }
    private void OnConsumableTriggered(InputAction.CallbackContext context)
    {
        consumableInteracted = context.ReadValueAsButton();
    }
}

    