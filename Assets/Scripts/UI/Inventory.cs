using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryMenu;
    public PlayerInputActions inputActions;
    public GameObject Text;
    public GameObject Image;
    private Item Item;

    public bool interacted;
    void Awake()
    {
        Item = null;
        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Inventory.started += OnInventoryTriggered;

    }
    private void Update()
    {
        if (interacted)
        {
            if (inventoryMenu.activeSelf)
                inventoryMenu.SetActive(false);
            else
                inventoryMenu.SetActive(true);
            interacted= false;
        }
    }
    public void ShowInfo(Item item)
    {
        Item = item;
        Text.GetComponent<TextMeshProUGUI>().text = item.itemDescription;
        Image.GetComponent<UnityEngine.UI.Image>().sprite = item.icon;
    }

    public void Equip()
    {
        if(!Item.isEquiped)
            Item.isEquiped = true;
        else
            Item.isEquiped = false;
    }

    private void OnInventoryTriggered(InputAction.CallbackContext context)
    {
        interacted = context.ReadValueAsButton();
    }
}
