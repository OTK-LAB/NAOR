using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeControl : MonoBehaviour
{

    public GameObject player;
    public Transform positionAfterBridge;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.GetComponent<NextScene>().isEntered())
        {
            player.transform.position = positionAfterBridge.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
