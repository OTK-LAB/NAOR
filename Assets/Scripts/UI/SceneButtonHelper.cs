using UnityEngine;
using UnityEngine.UI;

namespace NAOR.UI
{
    [RequireComponent(typeof(Button))]
    public class SceneButtonHelper : MonoBehaviour
    {
        public string sceneName;
        public SceneSelectionUI uiScript;

        private void Start()
        {
            Button btn = GetComponent<Button>();
            if (btn != null && uiScript != null && !string.IsNullOrEmpty(sceneName))
            {
                btn.onClick.AddListener(() => uiScript.LoadPrototypeScene(sceneName));
            }
        }
    }
}
