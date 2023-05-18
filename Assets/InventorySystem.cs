using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public PlayerInputActions inputActions;
    public InventoryScriptable playerInventory;
    public InventoryScriptable shopInventory;
    public Inventory PlayerInventoryManager;
    public Inventory ShopInventoryManager;

    public bool interacted;
    public Item item;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Consumable.started += OnConsumableTriggered;

    }
    private void Start()
    {

    }
    public void Buy()
    {
        item = ShopInventoryManager.GetSelectedItem();
        playerInventory.AddItem(item);
        shopInventory.RemoveItem(item);
    }
    private void OnConsumableTriggered(InputAction.CallbackContext context)
    {
        interacted = context.ReadValueAsButton();
    }
}

    