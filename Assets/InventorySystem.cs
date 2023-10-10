using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UltimateCC;
using System.Collections.Concurrent;

public class InventorySystem : MonoBehaviour
{
    public HealthSystem HealthSystem;
    public ManaSoulSystem ManaSoulSystem;
    public PlayerInputActions inputActions;
    public InventoryScriptable playerInventory;
    public InventoryScriptable shopInventory;
    public Inventory PlayerInventoryManager;
    public Inventory ShopInventoryManager;
    public PlayerMain player;
    public CurrencyScript Currency;

    public bool consumableInteracted;
    public Item equipedItem;
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
            //HealthSystem.broccoli = false;
        }
        else if(_selectedItem.id == 4)
        {
            player.PlayerData.Shop.HorizontalSpeedMultiplier = 1;
        }
        else if(_selectedItem.id == 5)
        {
            player.PlayerData.Shop.AttackMultiplier= 1;
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
        equipedItem = PlayerInventoryManager.GetEquipedItem();
        shopSelectedItem = ShopInventoryManager.GetSelectedItem();
        selectedItem = PlayerInventoryManager.GetSelectedItem();
        if (consumableInteracted && equipedItem != playerInventory.nullitem && !equipedItem.inDelay)
        {
            RemovePlayerItem(Consume(equipedItem));
        }
        consumableInteracted = false;
    }
    public void RemovePlayerItem(bool result)
    {
        if (result)
        {
            playerInventory.RemoveItem(equipedItem);
            if (equipedItem.stack == 0) { PlayerInventoryManager.WhenRemoved(); }
        }
    }
    public bool Consume(Item _equipedItem)
    {

        switch (_equipedItem.id)
        {
            case 1:
                return Apple(_equipedItem);
            case 2:
                //return Broccoli(_equipedItem);
            case 3:
                return SpringWater(_equipedItem);
            case 4:
                return EnergyDrink(_equipedItem);
            case 5:
                return Spinach(_equipedItem);

            default: break;
        }
        return false;
    }
    public void PermanentConsume(Item _shopSelectedItem)
    {
        switch (_shopSelectedItem.id) 
        { 
            case 6:
                HpBoost(_shopSelectedItem);
                break;
            case 7:
                ManaBoost(_shopSelectedItem);
                break;
            case 8:
                AbilityPowerBoost(_shopSelectedItem);
                break;

            default: break;
        }
        playerInventory.RemoveItem(_shopSelectedItem);
    }

    public bool Apple(Item _selectedItem)
    {
        if (HealthSystem.CurrentHealth < HealthSystem.MaxHealth)
        {
            HealthSystem.Heal(_selectedItem.value);
            return true;
        }
        return false;
    }
    public bool Broccoli(Item _selectedItem)
    {
        //HealthSystem.broccoli=true;
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
        return true;

    }
    public bool SpringWater(Item _selectedItem)
    {
        if (ManaSoulSystem.currentMana < ManaSoulSystem.maxMana)
        {
            ManaSoulSystem.AddMana(33);
            return true;
        }
        return false;
    }
    public bool EnergyDrink(Item _selectedItem)
    {
        player.PlayerData.Shop.HorizontalSpeedMultiplier = _selectedItem.value;
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
        return true;
    }
    public bool Spinach(Item _selectedItem)
    {
        float value = _selectedItem.value;
        player.PlayerData.Shop.AttackMultiplier = value;
        StartCoroutine(EffectCoroutine(_selectedItem));
        StartCoroutine(DelayCoroutine(_selectedItem));
        return true;
    }
    public void HpBoost(Item _selectedItem)
    {
        HealthSystem.MaxHealth+=_selectedItem.value;
    }
    public void ManaBoost(Item _selectedItem)
    {
        ManaSoulSystem.maxMana+=_selectedItem.value;
        ManaSoulSystem.manaBar.SetMaxValue(ManaSoulSystem.maxMana);

    }
    public void AbilityPowerBoost(Item _selectedItem)
    {
        player.PlayerData.Shop.AbilityPowerMultiplier= _selectedItem.value;
    }
    public void Buy() 
    {
        if (playerInventory.CanBuy(shopSelectedItem))
        {
            if (Currency.SpendMoney(shopSelectedItem.price, 2))
            {
                playerInventory.AddItem(shopSelectedItem);
                shopSelectedItem.shopStack--;
                if (shopSelectedItem.type == "permanent")
                {
                    PermanentConsume(shopSelectedItem);
                }
            }
        }
    }
    private void OnConsumableTriggered(InputAction.CallbackContext context)
    {
        consumableInteracted = context.ReadValueAsButton();
    }
}

    