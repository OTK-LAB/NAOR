using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIScript : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;
    public GameObject inGameUI;
    public GameObject Player;

    public float time = 2f;
    bool fadein = false;

    public Image image;

    public PlayerInputActions inputActions;

    private void Awake()
    {
        if (Player != null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Interaction.Enable();

            //inputActions.Interaction.NpcInteraction.started += OnDialougeTriggered;
        }
    }

    public void EnableInteraction()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<UltimateCC.PlayerInputManager>().playerControls.Enable();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<UltimateCC.PlayerInputManager>().playerControls.Enable();
                Resume();
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<UltimateCC.PlayerInputManager>().playerControls.Disable();
                Pause();
            }
        }

        else if (fadein)
        {
            Time.timeScale = 1.0f;
            StartCoroutine(FadeIn());
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        inGameUI.SetActive(true);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    void Pause()
    {
        inGameUI.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadScene()
    {
        Time.timeScale = 1.0f;
        fadein = true;
        StartCoroutine(Load());
    }

    public void LoadNextScene()
    {
        fadein = true;
        StartCoroutine(LoadNext());

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator Load()
    {

        yield return new WaitForSeconds(3);
        fadein = false;
        SceneManager.LoadScene(0);
    }


    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(3);
        fadein = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }



    public IEnumerator FadeIn()
    {

        var tempColor = image.color;
        for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            tempColor = image.color;
            tempColor.a = Mathf.Lerp(0f, 1f, f / 2);
            image.color = tempColor;
            yield return null;
        }
        fadein = false;
        tempColor = image.color;
        tempColor.a = 1;
        image.color = tempColor;
    }

}
