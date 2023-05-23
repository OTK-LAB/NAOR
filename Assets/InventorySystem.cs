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
    public PlayerData playerData;


    public bool consumableInteracted;
    public Item selectedItem;

    private bool indicator;
    public delegate void myFunciton();
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Consumable.started += OnConsumableTriggered;

        selectedItem = playerInventory.nullitem;
    }
    private void Start()
    {
        StartCoroutine(DelayCoroutine(0));
    }
    IEnumerator DelayCoroutine(float time)
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSeconds(10);
        Debug.Log("Coroutine resumed after 3 seconds");
    }
    private void Update()
    {
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
                Broccoli(selectedItem,true);
                break;
            case 3:
                SpringWater(selectedItem);
                break;
            case 4:
                EnergyDrink(selectedItem,true);
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
    public void Broccoli(Item _selectedItem,bool indicator)
    {
        HealthSystem.broccoli=true;
        //  if (indicator)
        DelayCoroutine(10f);
        HealthSystem.broccoli=false;
        Debug.Log("brokk");
        //DelayCoroutine(_selectedItem.time);
        //Broccoli(selectedItem, false);
        //HealthSystem.broccoli = !indicator;
        
    }
    public void SpringWater(Item _selectedItem)
    {
        ManaSoulSystem.AddMana(33);
    }
    public void EnergyDrink(Item _selectedItem,bool indicator)
    {
        HealthSystem.broccoli = indicator;
        if (indicator)
        {
            DelayCoroutine(_selectedItem.time);
            EnergyDrink(selectedItem, false);
        }
    }
    public void Spinach(Item _selectedItem)
    {

    }
    public void Buy()
    {
        if (playerInventory.AddItem(selectedItem))
            shopInventory.RemoveItem(selectedItem);
        
    }
    private void OnConsumableTriggered(InputAction.CallbackContext context)
    {
        consumableInteracted = context.ReadValueAsButton();
    }
}

    