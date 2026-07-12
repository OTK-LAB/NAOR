using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack : MonoBehaviour
{
    private Stack<GameObject> itemStack;
    private GameObject itemToStack;
    private int itemAmount;

    void Start()
    {
        itemStack = new Stack<GameObject>();
        UpdateStack(itemAmount);
    }
    
    public void SetItem(GameObject obj, int amount)
    {
        this.itemToStack = obj;
        this.itemAmount = amount; 
    }

    public void UpdateStack(int amount)
    {
        GameObject tmp;
        for(int i = 0; i < amount; i++)
        {
            tmp = Instantiate(itemToStack);
            tmp.SetActive(false);
            this.itemStack.Push(tmp);
        }
    }

    public void PushToStack(GameObject item)
    {
        this.itemStack.Push(item);
    }
    public GameObject PopFromStack()
    {
        if(itemStack.Count > 0)
        {
            Debug.Log("An Item Popped From Stack!");            
            return itemStack.Pop();
        }
        else
        {
            return null;
        }
    }

    public Stack<GameObject> GetStack()
    {
        return this.itemStack;
    }
}
