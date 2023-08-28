using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public GameObject shadow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("ShadowSwap");
        }

    }

    public IEnumerator ShadowSwap()
    {
        Debug.Log("test");
        while (shadow.GetComponent<SpriteRenderer>().color.a!=0)
        {
            float acolor=Mathf.MoveTowards(shadow.GetComponent<SpriteRenderer>().color.a, 0, 0.01f);
            shadow.GetComponent<SpriteRenderer>().color = new Color(1,1,1,acolor);
            yield return new WaitForFixedUpdate();
        }
    }


}
