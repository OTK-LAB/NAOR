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
    public Cinemachine.CinemachineVirtualCamera vcam;

    public bool deployNuke = false;
    private bool nukeDeployed = false;
    private bool nuking;

    public GameObject dustWave;

    void Start()
    {
        audioSource = audioSourceObject.transform.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(deployNuke)
        {
            StartCoroutine(Nuke());
            //audioSource.PlayOneShot(meteorSFX);
            GameObject vfxmeteor = Instantiate(meteorVFX) as GameObject;
            Destroy(vfxmeteor, nukeDelay);
            deployNuke = false;
        }
        if (nuking && vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain < 4.1f)
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain += Time.deltaTime / 2;
        else if (!nuking && vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain > 0)
        {
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain -= Time.deltaTime * 2;
            if (vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain < 0)
                vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !nukeDeployed)
        {
            deployNuke = true;
            nukeDeployed = true;
        }
    }

    IEnumerator Nuke()
    {
        yield return new WaitForSeconds(nukeDelay);
        nuking = true;
        dustWave.SetActive(true);
        audioSource.PlayOneShot(nukeSFX);
        GameObject vfxNuke = Instantiate(nukeVFX) as GameObject;
        Destroy(vfxNuke, 10);
        Invoke("DisableNuke", 9);
    }

    void DisableNuke()
    {
        nuking = false;
    }
}
