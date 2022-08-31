using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ///<summary>
        ///Esc key will pause or unpause the game
        ///</summary>
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    ///<summary>
    ///Makes the pause menu invisible and unfreezez the game
    ///</summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    ///<summary>
    ///Makes the pause menu visible and freezez the game
    ///Note: We may need to find an alternative to Time.timescale function to freeze te game if we want to keep certain aanimations running
    /// </summary>

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    /// <summary>
    /// Quits the game entirely
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}