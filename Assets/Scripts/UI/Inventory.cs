using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public InventoryScriptable inventory;
    public GameObject inventoryMenu;
    public PlayerInputActions inputActions;
    public GameObject Text;
    public GameObject Image;
    public GameObject Grid;
    public Item selectedItem;
    public GameObject equipedItemImage;

    public bool PlayerInventory;
    public bool interacted;
    public Item equipedItem;

    void Awake()
    {
        
        inventory.RefreshItems();
        selectedItem = inventory.nullitem;
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();
        equipedItem= inventory.nullitem;
        inputActions.UI.Inventory.started += OnInventoryTriggered;
        DefaultValues();

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
    public void DefaultValues()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] != inventory.nullitem)
            {
                inventory.items[i].isEquiped = false;
                inventory.items[i].inDelay = false;
            }
        }
    }

    public void UpdateInfo()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] != inventory.nullitem)
            {
                if (PlayerInventory)
                {
                    Grid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                    Grid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].stack.ToString();
                }
                else
                {
                    Grid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                    Grid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].shopStack.ToString();
                }
                    
            }
            else
            {
                Grid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
                Grid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        if (PlayerInventory)
        {
            equipedItemImage.GetComponent<UnityEngine.UI.Image>().sprite = equipedItem.icon;
            equipedItemImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = equipedItem.stack.ToString();
        }
    }
    public void SelectItem(int slot)
    {
        selectedItem = inventory.items[slot];
        Image.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[slot].icon;
        Text.GetComponent<TextMeshProUGUI>().text = inventory.items[slot].itemDescription;
    }
    public void WhenRemoved()
    {
        equipedItem = inventory.nullitem;
        Image.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
        Text.GetComponent<TextMeshProUGUI>().text = inventory.nullitem.itemDescription;
    }
    public Item GetSelectedItem()
    {
        return selectedItem;
    }
    public Item GetEquipedItem()
    {
        return equipedItem;
    }
    public void OpenShop()
    {
        inventoryMenu.SetActive(true);
    }
    public void CloseShop()
    {
        inventoryMenu.SetActive(false);
    }
    public void Equip()
    {
        if (selectedItem.isEquiped)
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            equipedItem= inventory.nullitem;
            equipedItem.isEquiped =false;
        }
        else
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            equipedItem = selectedItem;
            equipedItem.isEquiped=true;
            Debug.Log("naberknk");
        }
    }

    private void OnInventoryTriggered(InputAction.CallbackContext context)
    {
        interacted = context.ReadValueAsButton();
    }
}
