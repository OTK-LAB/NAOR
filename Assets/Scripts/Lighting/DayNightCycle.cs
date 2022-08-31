using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{

    private Camera cam;
    public float cycleTime;
    private Color dayColor;
    public Color dayTint;
    public Color nightTint;
    public Light2D globalLight;
    public VolumeProfile volume;
    public SpriteRenderer sun;

    public SpriteRenderer[] stars;
    public Light2D[] lights;

    void Start()
    {
        cam = GetComponent<Camera>();
        dayColor = cam.backgroundColor;
        if (volume.TryGet<Bloom>(out Bloom bloom))
        {
            bloom.tint.value = dayTint;
        }
    }

    void Update()
    {
        cam.backgroundColor = Color.Lerp(dayColor, Color.black, Mathf.PingPong(Time.time/cycleTime, 1));
        globalLight.intensity = Mathf.Lerp(0.9f, 0.6f, Mathf.PingPong(Time.time / cycleTime, 1));
        sun.color = new Color(sun.color.r, sun.color.g, sun.color.b, Mathf.Lerp(1f, 0f, Mathf.PingPong(Time.time / cycleTime, 1)));
        
        foreach (var star in stars)
            star.color = new Color(star.color.r, star.color.g, star.color.b, Mathf.Lerp(0f, 1f, Mathf.PingPong(Time.time / cycleTime, 1)));
            
        if(volume.TryGet<Bloom>(out Bloom bloom))
        {
            bloom.tint.value = Color.Lerp(dayTint, nightTint, Mathf.PingPong(Time.time / cycleTime, 1));
        }

        foreach (var light in lights)
            light.intensity = Mathf.Lerp(0.15f, 0f, Mathf.PingPong(Time.time / cycleTime, 1));
    }
}
