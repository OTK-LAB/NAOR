using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect;
    public float offset;
    
    void Start() 
    {
        startPos = transform.position.x;
        /*length = GetComponent<SpriteRenderer>().bounds.size.x;*/
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1-parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);
        //transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
        transform.position = new Vector3((cam.transform.position.x * parallaxEffect) + offset, transform.position.y, transform.position.z);
        /*if(temp > startPos + length)
            startPos += length;
        else if (temp < startPos - length)
            startPos -= length;*/ 
         //transform.position += new Vector3(-parallaxEffect * Time.deltaTime, 0, 0);
    }
}
