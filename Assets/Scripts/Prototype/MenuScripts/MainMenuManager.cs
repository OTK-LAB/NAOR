using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    public GameObject IntroductionMenu;
    public GameObject MainMenu;
    public GameObject Menu;
    public GameObject StartMenu;
    public GameObject ContinueMenu;
    public GameObject SettingsMenu;
    public GameObject ExitMenu;
    public GameObject PrologueCanvas;
    public GameObject MenuCanvas;
    public AudioSource audioSource;
    public AudioClip Seslendirme;
    public void Start()
    {
        audioSource = audioSource.GetComponent<AudioSource>();
    }


    public void Update()
    {
        if (Input.anyKeyDown && IntroductionMenu.activeInHierarchy)
        {
            IntroductionMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    public void StartScreen()
    {
        Menu.SetActive(false);
        StartMenu.SetActive(true);
    }
   public void Transition()
    {
        PrologueCanvas.SetActive(true);
        audioSource.clip = Seslendirme;
        audioSource.PlayOneShot(Seslendirme);
        Destroy(MenuCanvas);
        // yield return new WaitForSeconds(5);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Loads the next scene in line
    }
    public void StartGame(){
        Transition();
    }
    
    public void Continue()
    {
        ContinueMenu.SetActive(true);
        Menu.SetActive(false);
    }
    public void Settings()
    {
        SettingsMenu.SetActive(true);
        Menu.SetActive(false);
    }

    public void ExitScreen()
    {
        ExitMenu.SetActive(true);
        Menu.SetActive(false);
    }
    public void EnlargeButton(Button button)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        float y = rt.offsetMax.y;
        rt.offsetMax = new Vector2(240f, y);
    }

    public void ResetButton(Button button)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        float y = rt.offsetMax.y;
        rt.offsetMax = new Vector2(160f, y);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

 
}
