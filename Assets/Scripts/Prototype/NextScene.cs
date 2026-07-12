using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    private Scene _scene;
    public string sceneName;
    public static bool entered = false;

    private void Awake() {
        _scene = SceneManager.GetActiveScene();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
            entered = true;
        }
    }

    public bool isEntered(){
        return entered;
    }
}
