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
    //public HealthSystem HealthSystem;
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
    public GameObject dagger;
    public GameObject iceDagger;
    public Item consumableEquipedItem;
    public Item throwableEquipedItem;
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
        consumableEquipedItem = PlayerInventoryManager.GetConsumableEquipedItem();
        throwableEquipedItem = PlayerInventoryManager.GetThrowableEquipedItem();
        shopSelectedItem = ShopInventoryManager.GetSelectedItem();
        selectedItem = PlayerInventoryManager.GetSelectedItem();
        if (consumableEquipedItem != playerInventory.nullitem && !consumableEquipedItem.inDelay)
        {
            if (consumableInteracted) { RemovePlayerItem(Consume(consumableEquipedItem),0); }
        }
        if (throwableEquipedItem != playerInventory.nullitem && !throwableEquipedItem.inDelay)
        {
            if (throwableInteracted) { RemovePlayerItem(Throw(throwableEquipedItem),1); }
        }
        consumableInteracted = false;
        throwableInteracted = false;
    }
    public void RemovePlayerItem(bool result,int type)
    {
        //0 consumable 1 throwable
        if (result)
        {
            if (type==0)
            {
                playerInventory.RemoveItem(consumableEquipedItem);
                if (consumableEquipedItem.stack == 0) { PlayerInventoryManager.WhenRemoved(0); }
            }
            else
            {
                playerInventory.RemoveItem(throwableEquipedItem);
                if (throwableEquipedItem.stack == 0) { PlayerInventoryManager.WhenRemoved(1); }
            }
           
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
            case 8:
                return Dagger(_equipedItem);
            case 9:
                return IceDagger(_equipedItem);

            default: break;
        }
        return false;
    }

    

    public bool Apple(Item _equipedItem)
    {
        if (PlayerMain.Instance.PlayerData.healthSystem.CurrentHealth < PlayerMain.Instance.PlayerData.healthSystem.MaxHealth)
        {
            PlayerMain.Instance.PlayerData.healthSystem.Heal(_equipedItem.value);
            return true;
        }
        return false;
    }
    public bool Broccoli(Item _equipedItem)
    {
        PlayerMain.Instance.PlayerData.healthSystem.DamageMultiplier = _equipedItem.value;
        StartCoroutine(EffectCoroutine(_equipedItem));
        StartCoroutine(DelayCoroutine(_equipedItem));
        return true;

    }
    public bool SpringWater(Item _equipedItem)
    {
        if (ManaSoulSystem.CurrentMana < ManaSoulSystem.MaxMana)
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
    public bool Dagger(Item _selectedItem)
    {
        Instantiate(dagger, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        return true;
    }
    public bool IceDagger(Item _selectedItem)
    {
        Instantiate(iceDagger, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        return true;
    }

    public void HpBoost(Item _selectedItem)
    {
        float newMaxHealth = PlayerMain.Instance.PlayerData.healthSystem.MaxHealth + _selectedItem.value;
        PlayerMain.Instance.PlayerData.healthSystem.MaxHealth = newMaxHealth;
    }
    public void ManaBoost(Item _selectedItem)
    {
        float newMaxMana = ManaSoulSystem.MaxMana + _selectedItem.value;
        ManaSoulSystem.MaxMana = newMaxMana;
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
                playerInventory.ArrangeItems();
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

    