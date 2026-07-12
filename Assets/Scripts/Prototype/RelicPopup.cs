using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicPopup : MonoBehaviour
{
    public GameObject RelicMenu;
    public GameObject Popup;
    bool popupToggle = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(RelicMenu.activeInHierarchy && !popupToggle)
        {
            
            Popup.SetActive(true);
            popupToggle = true;
        }
        if(!RelicMenu.activeInHierarchy && Popup.activeInHierarchy)
        {
            Popup.SetActive(false);
        }
    }

    public void ClosePopup()
    {
        Popup.SetActive(false);
    }
}
