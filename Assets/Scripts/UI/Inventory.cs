using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public InventoryScriptable inventory;
    public GameObject inventoryMenu;
    public PlayerInputActions inputActions;
    public GameObject Text;
    public GameObject Image;
    public GameObject Grid;
    public Item selectedItem;

    public bool PlayerInventory;
    public bool interacted;
    private Item lastSelectedItem;
    void Awake()
    {
        
        inventory.RefreshItems();
        selectedItem = inventory.nullitem;
        lastSelectedItem = inventory.nullitem;
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Inventory.started += OnInventoryTriggered;

    }
    private void Update()
    {
        
        if (interacted && PlayerInventory)
        {
            if (inventoryMenu.activeSelf)
                inventoryMenu.SetActive(false);
            else
                inventoryMenu.SetActive(true);
            interacted = false;
        }
        UpdateInfo();
    }
    public void UpdateInfo()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] != inventory.nullitem)
            {
                Grid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                Grid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].stack.ToString();
            }
            else
            {
                Grid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
                Grid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
    public void SelectItem(int slot)
    {
        foreach (Item item in inventory.items) { item.isEquiped = false; }

        selectedItem = inventory.items[slot];
        
        Image.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[slot].icon;
        Text.GetComponent<TextMeshProUGUI>().text = inventory.items[slot].itemDescription;
      
    }
    public Item GetSelectedItem()
    {
        return selectedItem;
    }
    public void OpenShop()
    {
        inventoryMenu.SetActive(true);
    }
    public void Equip()
    {
        
        if (selectedItem.isEquiped)
        {
            selectedItem.isEquiped =false;
        }
        else
        {
            selectedItem.isEquiped = true;
        }
    }

    private void OnInventoryTriggered(InputAction.CallbackContext context)
    {
        interacted = context.ReadValueAsButton();
    }
}
