using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public static PortalController instance;

    [SerializeField] private GameObject portal1, portal2;

    [SerializeField] private Transform portal1SpawnPoint, portal2SpawnPoint;

    private Collider2D portal1Collider, portal2Collider;

    [SerializeField] private GameObject clone;

    [HideInInspector] public GameObject instantiatedClone;



    void Start()
    {
        instance = this;
        portal1Collider = portal1.GetComponent<Collider2D>();
        portal2Collider = portal2.GetComponent<Collider2D>();
    }

    public void CreateClone(string whereToCreate)
    {
        if (whereToCreate == "atPortal1")
        {
            instantiatedClone = Instantiate(clone, portal1SpawnPoint.position, Quaternion.identity);
        }
        else if (whereToCreate == "atPortal2")
        {
            instantiatedClone = Instantiate(clone, portal2SpawnPoint.position, Quaternion.identity);
        }
    }

    public void DisableCollider(string colliderToDisable)
    {
        if (colliderToDisable == "portal1")
        {
            portal1Collider.enabled = false;
        }
        else if (colliderToDisable == "portal2")
        {
            portal2Collider.enabled = false;
        }
    }

    public void EnableColliders()
    {
        portal1Collider.enabled = true;
        portal2Collider.enabled = true;
    }
}
