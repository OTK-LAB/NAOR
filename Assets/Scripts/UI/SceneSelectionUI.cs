using UnityEngine;
using UnityEngine.SceneManagement;

namespace NAOR.UI
{
    public class SceneSelectionUI : MonoBehaviour
    {
        public void LoadPrototypeScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log($"[SceneSelectionUI] Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("[SceneSelectionUI] Attempted to load a scene with an empty name.");
            }
        }
    }
}
