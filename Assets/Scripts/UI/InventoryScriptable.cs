using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Create New Inventory")]
public class InventoryScriptable : ScriptableObject
{
    public int maxItems = 16;
    public List<Item> items = new();
    public bool AddItem(Item itemToAdd)
    {
        // Finds an empty slot if there is one
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToAdd&& items[i].number < items[i].stackSize)
            {
                items[i].number++;
                Debug.Log("Add item number");
                return true;
            }
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                Debug.Log("Add item null");
                return true;
            }
        }
        // Adds a new item if the inventory has space
        if (items.Count < maxItems)
        {
            items.Add(itemToAdd);
            Debug.Log("Add item");
            return true;
        }
        Debug.Log("No space in the inventory");
        return false;
    }
    public bool RemoveItem(Item itemToRemove)
    {
        // Finds an item slot which one will remove
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                Debug.Log("Remove item");
                return true;
            }
        }
        Debug.Log("No item in the inventory for remove");
        return false;
    }
    public void ShowItem()
    {

    }
}
