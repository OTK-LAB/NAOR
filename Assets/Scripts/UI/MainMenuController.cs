using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "CombatImprovementTest"; // Example
    [SerializeField] private GameObject optionsPanel;

    // ponytail: keep it simple, direct scene load. No async loader needed unless scenes are massive.
    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested");
        Application.Quit();
    }
}
