using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArmScript : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject parchment;
    [SerializeField] private GameObject text;
    public bool inRange;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.Play("TutorialArmSpawn");
            inRange = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRange)
        {
            Debug.Log("e pressed");
            parchment.SetActive(!parchment.activeInHierarchy);
            text.SetActive(!text.activeInHierarchy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.Play("TutorialArmDespawn");
            parchment.SetActive(false);
            text.SetActive(false);
            inRange = false;
        }
    }

    public void ChangeAnimationToSIdle()
    {
        animator.Play("TutorialArmSpawnedIdle");
    }

    public void ChangeAnimationToIdle()
    {
        animator.Play("TutorialArmIdle");
    }
}
