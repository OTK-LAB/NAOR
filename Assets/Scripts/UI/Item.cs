
using UnityEngine;

[CreateAssetMenu (fileName="New Item", menuName ="Inventory/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public Sprite icon;
    public string itemDescription;
    public bool isEquiped;
    public float time;
    public int stack;
    public int stackSize;
    public string type;

    public Item()
    {
        id = 0;
        itemName = "";
        value = 0;
        icon = null;
        itemDescription = "";
        isEquiped = false;
        time = 0;
        stack = 1;
        stackSize = 3;
        type = "";
    }
}
