using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointMenuScript : MonoBehaviour
{
    public GameObject AbilitiesButton;
    public GameObject RelicsButton;
    public GameObject SkillTreeMenu;
    public GameObject ShallowGraveMenu;
    public GameObject CheckPointMenu;
    public static bool isMenuOpen = false;

    // Update is called once per frame

    private void Start()
    {
        this.enabled = false;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            if (isMenuOpen)
            {
                ResetTabs();
                CloseCheckPointMenu();
            }
            else
            {
                OpenCheckPointMenu();
            }
        }
    }

    void ResetTabs()
    {
        AbilitiesButton.SetActive(false);
        RelicsButton.SetActive(false);
        SkillTreeMenu.SetActive(false);
        ShallowGraveMenu.SetActive(true);
    }
    void OpenCheckPointMenu()
    {
        ResetTabs();
        CheckPointMenu.SetActive(true);
        TogglePause();
        isMenuOpen = true;
    }

    public void CloseCheckPointMenu()
    {
        TogglePause();
        CheckPointMenu.SetActive(false);
        isMenuOpen = false;
    }
    public void TogglePause()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }

        else if(Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
    }
}
