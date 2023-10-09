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
    public bool throwableInteracted;
    public GameObject bomb;
    public GameObject slowBomb;
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

        inputActions.UI.Throwable.started += OnThrowableTriggered;
        inputActions.UI.Throwable.performed += OnThrowableTriggered;
        inputActions.UI.Throwable.canceled += OnThrowableTriggered;

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
        if (equipedItem != playerInventory.nullitem && !equipedItem.inDelay)
        {
            if (consumableInteracted) { RemovePlayerItem(Consume(equipedItem)); }
            if (throwableInteracted) { RemovePlayerItem(Throw(equipedItem)); }
        }
        consumableInteracted = false;
        throwableInteracted = false;
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
                return Broccoli(_equipedItem);
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
            case 13:
                HpBoost(_shopSelectedItem);
                break;
            case 14:
                ManaBoost(_shopSelectedItem);
                break;
            case 15:
                AbilityPowerBoost(_shopSelectedItem);
                break;

            default: break;
        }
        playerInventory.RemoveItem(_shopSelectedItem);
    }
    public bool Throw(Item _equipedItem)
    {
        switch (_equipedItem.id)
        {
            case 6:
                return Bomb(_equipedItem);
            case 7:
                return SlowBomb(_equipedItem);

            default: break;
        }
        return false;
    }

    

    public bool Apple(Item _equipedItem)
    {
        if (HealthSystem.currentHealth < HealthSystem.maxHealth)
        {
            HealthSystem.Heal(_equipedItem.value);
            return true;
        }
        return false;
    }
    public bool Broccoli(Item _equipedItem)
    {
        HealthSystem.broccoli=true;
        StartCoroutine(EffectCoroutine(_equipedItem));
        StartCoroutine(DelayCoroutine(_equipedItem));
        return true;

    }
    public bool SpringWater(Item _equipedItem)
    {
        if (ManaSoulSystem.currentMana < ManaSoulSystem.maxMana)
        {
            ManaSoulSystem.AddMana(33);
            return true;
        }
        return false;
    }
    public bool EnergyDrink(Item _equipedItem)
    {
        player.PlayerData.Shop.HorizontalSpeedMultiplier = _equipedItem.value;
        StartCoroutine(EffectCoroutine(_equipedItem));
        StartCoroutine(DelayCoroutine(_equipedItem));
        return true;
    }
    public bool Spinach(Item _equipedItem)
    {
        float value = _equipedItem.value;
        player.PlayerData.Shop.AttackMultiplier = value;
        StartCoroutine(EffectCoroutine(_equipedItem));
        StartCoroutine(DelayCoroutine(_equipedItem));
        return true;
    }

    public bool Bomb(Item _selectedItem)
    {
        Instantiate(bomb, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        return true;
    }
    public bool SlowBomb(Item _selectedItem)
    {
        Instantiate(slowBomb, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        return true;
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
    private void OnThrowableTriggered(InputAction.CallbackContext context)
    {
        throwableInteracted = context.ReadValueAsButton();
    }
}

    