using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject Tabs;
    public GameObject InventoryTab;
    public GameObject AbilityTab;

    public GameObject ConsumableGrid;
    public GameObject ThrowableGrid;

    public GameObject DashGrid;

    private void Awake()
    {
        Tabs.SetActive(false);

        InventoryTab.SetActive(true);
        ConsumableGrid.SetActive(true);
        ThrowableGrid.SetActive(false);
        
        AbilityTab.SetActive(false);
        DashGrid.SetActive(true);
    }
    public void tabs() 
    {
        if (Tabs.activeSelf)
            Tabs.SetActive(false);
        else
            Tabs.SetActive(true);
    }public void inventoryTab() 
    {
        AbilityTab.SetActive(false);
        DashGrid.SetActive(false);

        InventoryTab.SetActive(true);
        ConsumableGrid.SetActive(true);
        ThrowableGrid.SetActive(false);
    }
    public void abilityTab() 
    {
        InventoryTab.SetActive(false);
        AbilityTab.SetActive(true);
        DashGrid.SetActive(true);
    }
    public void consumableGrid() 
    {
        ConsumableGrid.SetActive(true);
        ThrowableGrid.SetActive(false);
    }
    public void throwableGrid() 
    {
        ConsumableGrid.SetActive(false);
        ThrowableGrid.SetActive(true);
    }
    
    public void dashGrid() 
    {
        DashGrid.SetActive(true);
    }

}
