using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControl : MonoBehaviour
{
    public float        nukeDelay;
    public GameObject   meteor;
    public GameObject   nuke;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Nuke());
            GameObject vfxmeteor = Instantiate(meteor) as GameObject;
            Destroy(vfxmeteor, nukeDelay);
        }
    }

    IEnumerator Nuke()
    {
        yield return new WaitForSeconds(nukeDelay);
        GameObject vfxNuke = Instantiate(nuke) as GameObject;
        Destroy(vfxNuke, 10);
    }
}
