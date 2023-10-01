
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu (fileName="New Item", menuName ="Inventory/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public float value;
    public Sprite icon;
    public string itemDescription;
    public bool isEquiped;
    public bool inDelay;
    public float effectTime;
    public float delayTime;
    public int stack;
    public int shopStack;
    public int stackSize;
    public string type;
    public int price;

    public Item()
    {
        id = 0;
        itemName = "";
        value = 0;
        icon = null;
        itemDescription = "";
        isEquiped = false;
        inDelay = false;
        effectTime = 0;
        delayTime= 0;
        stack = 0;
        shopStack = 5;
        stackSize = 3;
        type = "";
        price = 0;
    }
}
