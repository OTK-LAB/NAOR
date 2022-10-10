using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public Animator _animator;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadNextLevel(int levelIndex)
    {
        _animator.Play("Crossfade_Start");
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(levelIndex);
    }
}
