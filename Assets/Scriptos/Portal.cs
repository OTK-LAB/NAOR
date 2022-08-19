using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool portal1;
    public bool portal2;

    private Rigidbody2D enteredRigidbody;
    private float enterVelocity, exitVelocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enteredRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        enterVelocity = enteredRigidbody.velocity.x;
        
        if (portal1)
        {
            PortalController.portalControllerInstance.DisableCollider("portal2");
            PortalController.portalControllerInstance.CreateClone("atPortal2");
        }
        else if (portal2)
        {
            PortalController.portalControllerInstance.DisableCollider("portal1");
            PortalController.portalControllerInstance.CreateClone("atPortal1");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        exitVelocity = enteredRigidbody.velocity.x;

        if (enterVelocity != exitVelocity)
        {
            Destroy(GameObject.Find("Clone"));
        }
        else if (gameObject.name != "Clone")
        {
            Destroy(collision.gameObject);
            PortalController.portalControllerInstance.EnableColliders();
            GameObject.Find("Clone").name = "Player";
        }
    }
}
