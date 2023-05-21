using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Create New Inventory")]
public class InventoryScriptable : ScriptableObject
{
    public int maxItems;
    public List<Item> items = new();
    public Item nullitem;
    
    public InventoryScriptable() { maxItems = 0; } 
    public void RefreshItems()
    {
        for (int i=0; i<maxItems; i++)
        {
            if (items.Count < maxItems) 
            {
                items.Add(nullitem);
            }
        }
    }
    public bool AddItem(Item itemToAdd)
    {
        // Finds an empty slot if there is one
        for (int i = 0; i < items.Count; i++)
        {
            if (itemToAdd.stack  < itemToAdd.stackSize)
            {
                if (items[i] == itemToAdd)
                {
                    items[i].stack++;
                    Debug.Log("Add item stack");
                    return true;
                }
                if (items[i] == nullitem)
                {
                    items[i] = itemToAdd;
                    Debug.Log("Add item null");
                    return true;
                }
            }
        }
        return false;
    }
    public bool RemoveItem(Item itemToRemove)
    {
        // Finds an item slot which one will remove
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = nullitem;
                Debug.Log("Remove item");
                return true;
            }
        }
        Debug.Log("No item in the inventory for remove");
        return false;
    }
    public bool Check(Item itemToAdd)
    {
        for (int i = 0; i < items.Count; i++) 
        {
            switch (items[i].type)
            {
                case "consumable":
                    if (items[i] == itemToAdd)
                    {
                        items[i].stack++;
                        Debug.Log("Add item stack");

                    }
                    return true;
                    
                default:
                    break;
            }

        }
        return false;
    }
}
