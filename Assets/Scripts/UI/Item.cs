
using UnityEngine;

[CreateAssetMenu (fileName="New Item", menuName ="Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public Sprite icon;
    public string itemDescription;
    public bool isEquiped;

    public Item()
    {
        id = 0;
        itemName = "";
        value = 0;
        icon = null;
        itemDescription = "";
        isEquiped = false;
    }
}
