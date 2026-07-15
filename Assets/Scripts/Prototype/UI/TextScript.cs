using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour
{
    public GameObject text;
    void Awake()
    {
        Invoke("ActivateText", 42f);
    }

    void ActivateText()
    {
        text.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
