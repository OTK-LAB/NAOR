using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ponytail: simple direct async load, no overengineered loading screens for now
            SceneManager.LoadSceneAsync(nextSceneName);
        }
    }
}
