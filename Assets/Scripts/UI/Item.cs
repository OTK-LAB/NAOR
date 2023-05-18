
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
    public int number;
    public int stackSize;

    public Item()
    {
        id = 0;
        itemName = "";
        value = 0;
        icon = null;
        itemDescription = "";
        isEquiped = false;
        time = 0;
        stackSize = 10;
    }
}
