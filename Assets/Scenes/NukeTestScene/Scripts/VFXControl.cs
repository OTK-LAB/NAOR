using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControl : MonoBehaviour
{
    public float        nukeDelay;
    public GameObject   meteorVFX;
    public GameObject   nukeVFX;
    public GameObject   audioSourceObject;
    public AudioClip    nukeSFX;
    //public AudioClip    meteorSFX;
    private AudioSource  audioSource;

    void Start()
    {
        audioSource = audioSourceObject.transform.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Nuke());
            //audioSource.PlayOneShot(meteorSFX);
            GameObject vfxmeteor = Instantiate(meteorVFX) as GameObject;
            Destroy(vfxmeteor, nukeDelay);
        }
    }

    IEnumerator Nuke()
    {
        yield return new WaitForSeconds(nukeDelay);
        audioSource.PlayOneShot(nukeSFX);
        GameObject vfxNuke = Instantiate(nukeVFX) as GameObject;
        Destroy(vfxNuke, 10);
    }
}
