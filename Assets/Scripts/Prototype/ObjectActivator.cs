using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectActivator : MonoBehaviour
{
    [SerializeField] string activatorTag = null;
    [SerializeField] bool deactivateOnExit = false;
    [SerializeField] GameObject[] objects = null;
    [SerializeField] GameObject[] objectsToDeact = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(activatorTag))
        {
            foreach (var obj in objects)
                obj.SetActive(true);
            foreach (var obj in objectsToDeact)
                obj.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (deactivateOnExit && collision.CompareTag(activatorTag))
        {
            foreach (var obj in objects)
                obj.SetActive(false);
        }
    }
}
