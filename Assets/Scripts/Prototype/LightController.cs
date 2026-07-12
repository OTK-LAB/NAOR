using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private Light2D torch;

    [SerializeField]
    private float intensityMin, intensityMax, flickerSpeed;

    float timer = 0;
    void Awake()
    {
        torch = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.CompareTag("fire"))
        {
            timer += Time.deltaTime;
            torch.intensity = Mathf.Lerp(intensityMin, intensityMax, Mathf.PingPong(timer * flickerSpeed, 1));
            Debug.Log("..");
        }
        else
            torch.intensity = Mathf.Lerp(intensityMin, intensityMax, Mathf.PingPong(Time.time * flickerSpeed, 1));
    }
}
