using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<CanvasRenderer>().SetAlpha(0.5f);
    }
}
