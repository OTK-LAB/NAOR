using UnityEngine;
using UnityEngine.SceneManagement;

namespace NAOR.Gameplay
{
    public class NativeSceneLoader : MonoBehaviour
    {
        [Tooltip("The name of the scene to load when triggered (if using physics triggers).")]
        public string triggerSceneName;

        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log($"[NativeSceneLoader] Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
        }

        public void ReturnToMainMenu()
        {
            Debug.Log("[NativeSceneLoader] Returning to DummyMainMenu");
            SceneManager.LoadScene("DummyMainMenu");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !string.IsNullOrEmpty(triggerSceneName))
            {
                LoadScene(triggerSceneName);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !string.IsNullOrEmpty(triggerSceneName))
            {
                LoadScene(triggerSceneName);
            }
        }
    }
}
