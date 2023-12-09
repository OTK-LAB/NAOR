using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Create New Inventory")]
public class InventoryScriptable : ScriptableObject
{
    public int maxItems;
    public List<Item> items = new();
    public Item nullitem;
    public int consumableMaxCount;
    public int permanentMaxCount;
    public int throwableMaxCount;
    public InventoryScriptable() { maxItems = 0; consumableMaxCount = 3; } 
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
    public void ArrangeItems()
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (items[i].id != i)
            {
                Arrange(items[i]);
                items[i] = nullitem;
            }
        }

    }
    private void Arrange(Item item)
    {
        Item tempitem = items[item.id];
        if(tempitem == item) { return; }
        items[item.id] = item;
        if (tempitem != null)
        {
            Arrange(tempitem);
        }
    }
    public bool CanBuy(Item itemToAdd)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (itemToAdd.shopStack <= 0)
            {
                return false;
            }
            if (items[i] == itemToAdd)
            {
                if (0 < itemToAdd.stack && itemToAdd.stack < itemToAdd.stackSize)
                {
                    return true;
                }
            }
            if (items[i] == nullitem && itemToAdd.stack == 0)
            {
                if (itemToAdd.type == "consumable")
                {
                    if (Count(itemToAdd.type) < consumableMaxCount)
                    {
                        return true;
                    }
                }
                else if (itemToAdd.type == "permanent")
                {
                    if (Count(itemToAdd.type) < permanentMaxCount)
                    {
                        return true;
                    }
                }
                else if (itemToAdd.type == "throwable")
                {
                    if (Count(itemToAdd.type) < throwableMaxCount)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
                if (itemToAdd.type == "consumable")
                {
                    if (Count(itemToAdd.type) < consumableMaxCount)
                    {
                        Debug.Log("cons " + Count(itemToAdd.type.ToString()));
                        items[i] = itemToAdd;
                        items[i].stack = 1;
                        return true;
                    }
                }
                else if (itemToAdd.type == "permanent")
                {
                    if (Count(itemToAdd.type) < permanentMaxCount)
                    {
                        Debug.Log("perm " + Count(itemToAdd.type.ToString()));
                        items[i] = itemToAdd;
                        items[i].stack = 1;
                        return true;
                    }
                }
                else if (itemToAdd.type == "throwable")
                {
                    if(Count(itemToAdd.type)< throwableMaxCount)
                    {
                        Debug.Log("throw " + Count(itemToAdd.type.ToString()));
                        items[i] = itemToAdd;
                        items[i].stack = 1;
                        return true;
                    }
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
                if (items[i].stack > 1)
                {
                    items[i].stack--;
                    Debug.Log("Romove stack");
                    return true;
                }
                else 
                {
                    items[i].stack--;
                    items[i].isEquiped = false;
                    items[i] = nullitem;
                    Debug.Log("Remove item");
                    return true;
                }
                
            }
        }
        return false;
    }
    public int Count(string itemType)
    {
        int count = 0;
        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i].type == itemType)
            {
                count++;
            }
        }
        return count;
    }
   
}
