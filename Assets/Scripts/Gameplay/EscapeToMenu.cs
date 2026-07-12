using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NAOR.Gameplay
{
    public class EscapeToMenu : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("[EscapeToMenu] Escape key pressed. Returning to DummyMainMenu.");
                SceneManager.LoadScene("DummyMainMenu");
            }
        }
    }
}
