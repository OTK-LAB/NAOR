using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool portal1;
    public bool portal2;

    private GameObject clone;

    private Rigidbody2D enteredRigidbody;
    private float enterVelocity, exitVelocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enteredRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        enterVelocity = enteredRigidbody.velocity.x;
        
        if (portal1)
        {
            PortalController.instance.DisableCollider("portal2");
            PortalController.instance.CreateClone("atPortal2");
        }
        else if (portal2)
        {
            PortalController.instance.DisableCollider("portal1");
            PortalController.instance.CreateClone("atPortal1");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        exitVelocity = enteredRigidbody.velocity.x;

        clone = PortalController.instance.instantiatedClone;

        if (enterVelocity != exitVelocity)
        {
            Destroy(clone);
        }
        else if (gameObject != clone)
        {
            collision.transform.position = clone.transform.position;
            PortalController.instance.EnableColliders();
            Destroy(clone);
        }
    }
}
