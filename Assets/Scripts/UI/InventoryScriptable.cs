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
            if (items[i] == itemToAdd)
            {
                
                if (0 < itemToAdd.stack && itemToAdd.stack < itemToAdd.stackSize)
                {
                    items[i].stack++;
                    Debug.Log("Add stack");
                    return true;
                }
            }
            if (items[i] == nullitem && itemToAdd.stack==0)
            {
                items[i] = itemToAdd;
                items[i].stack= 1;
                Debug.Log("Add item null");
                return true;
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
                if (items[i].stack > 1)
                {
                    items[i].stack--;
                    Debug.Log("Romove stack");
                    return true;
                }
                else 
                {
                    items[i] = nullitem;
                    Debug.Log("Remove item");
                    return true;
                }
                
            }
        }
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
                        return true;
                    }
                    break;
                default:
                    break;
            }

        }
        return false;
    }
}
