using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Microsoft.Unity.VisualStudio.Editor;

public class Inventory : MonoBehaviour
{
    public InventoryScriptable inventory;
    public GameObject Tabs;
    public PlayerInputActions inputActions;
    public GameObject Text;
    public GameObject Image;
    public GameObject shopGrid;
    public GameObject consumableGrid;
    public GameObject throwableGrid;
    public Item selectedItem;
    public GameObject consumableEquipedItemImage;
    public GameObject throwableEquipedItemImage;

    public bool PlayerInventory;
    public bool interacted;
    public Item consumableEquipedItem;
    public Item throwableEquipedItem;

    void Awake()
    {
        consumableEquipedItemImage.GetComponent<UnityEngine.UI.Image>().sprite= inventory.nullitem.icon;
        throwableEquipedItemImage.GetComponent<UnityEngine.UI.Image>().sprite= inventory.nullitem.icon;
        inventory.RefreshItems();
        inventory.ArrangeItems();
        selectedItem = inventory.nullitem;
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();
        consumableEquipedItem = inventory.nullitem;
        throwableEquipedItem = inventory.nullitem;
        inputActions.UI.Inventory.started += OnInventoryTriggered;
        DefaultValues();

    }
    private void Update()
    {
        if (interacted && PlayerInventory)
        {
            if (Tabs.activeSelf)
                Tabs.SetActive(false);
            else
                Tabs.SetActive(true);
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
                    if (1 <= inventory.items[i].id && inventory.items[i].id <= 5)
                    {
                        consumableGrid.transform.GetChild(0).GetChild(i-1).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                        consumableGrid.transform.GetChild(0).GetChild(i-1).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].stack.ToString();
                    }
                    else if (6 <= inventory.items[i].id && inventory.items[i].id <= 9)
                    {
                        throwableGrid.transform.GetChild(0).GetChild(i-6) .gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                        throwableGrid.transform.GetChild(0).GetChild(i-6).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].stack.ToString();
                    }
                }
                else
                {
                    shopGrid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[i].icon;
                    shopGrid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory.items[i].shopStack.ToString();
                }
                    
            }
            else
            {
                if (PlayerInventory)
                {
                    if (1 <= i && i <= 5)
                    {
                        consumableGrid.transform.GetChild(0).GetChild(i - 1).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
                        consumableGrid.transform.GetChild(0).GetChild(i - 1).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    }
                    else if (6 <= i && i <= 9)
                    {
                        throwableGrid.transform.GetChild(0).GetChild(i - 6).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
                        throwableGrid.transform.GetChild(0).GetChild(i - 6).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    }
                }
                else
                {
                    shopGrid.transform.GetChild(i).gameObject.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
                    shopGrid.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }
        if (PlayerInventory)
        {
            consumableEquipedItemImage.GetComponent<UnityEngine.UI.Image>().sprite = consumableEquipedItem.icon;
            consumableEquipedItemImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = consumableEquipedItem.stack.ToString();

            throwableEquipedItemImage.GetComponent<UnityEngine.UI.Image>().sprite = throwableEquipedItem.icon;
            throwableEquipedItemImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = throwableEquipedItem.stack.ToString();
        }
    }
    public void ShopSelectItem(int slot)
    {
        selectedItem = inventory.items[slot];
        Image.GetComponent<UnityEngine.UI.Image>().sprite = inventory.items[slot].icon;
        Text.GetComponent<TextMeshProUGUI>().text = inventory.items[slot].itemDescription;
    }
    public void SelectItem(int slot)
    {
        selectedItem = inventory.items[slot];
        if(1<= slot && slot <= 5) {
            consumableGrid.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inventory.items[slot].itemDescription;
            ConsumableEquip();
        }
        if(6<= slot && slot <= 9) {
            throwableGrid.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inventory.items[slot].itemDescription;
            ThrowableEquip(); 
        }
    }
    public void WhenRemoved(int type)
    {
        //0 consumable 1 throwable
        if (type ==0){
            consumableGrid.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inventory.nullitem.itemDescription;
            consumableEquipedItem = inventory.nullitem; 
        }
        else {
            throwableGrid.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inventory.nullitem.itemDescription;
            throwableEquipedItem = inventory.nullitem;
        }
        
       // Image.GetComponent<UnityEngine.UI.Image>().sprite = inventory.nullitem.icon;
       // Text.GetComponent<TextMeshProUGUI>().text = inventory.nullitem.itemDescription;
        UpdateInfo();
    }
    public Item GetSelectedItem()
    {
        return selectedItem;
    }
    public Item GetConsumableEquipedItem()
    {
        return consumableEquipedItem;
    }
    public Item GetThrowableEquipedItem()
    {
        return throwableEquipedItem;
    }
    public void OpenShop()
    {
        Tabs.SetActive(true);
    }
    public void CloseShop()
    {   
        Tabs.SetActive(false);
    }
    public void ConsumableEquip()
    {
        if (selectedItem.isEquiped)
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            consumableEquipedItem= inventory.nullitem;
            consumableEquipedItem.isEquiped =false;
        }
        else
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            consumableEquipedItem = selectedItem;
            consumableEquipedItem.isEquiped=true;
        }
    }
    public void ThrowableEquip()
    {
        if (selectedItem.isEquiped)
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            throwableEquipedItem = inventory.nullitem;
            throwableEquipedItem.isEquiped =false;
        }
        else
        {
            foreach (Item item in inventory.items) { item.isEquiped = false; }
            throwableEquipedItem = selectedItem;
            throwableEquipedItem.isEquiped=true;
        }
    }

    private void OnInventoryTriggered(InputAction.CallbackContext context)
    {
        interacted = context.ReadValueAsButton();
    }
}
