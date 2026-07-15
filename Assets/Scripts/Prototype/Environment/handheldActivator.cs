using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handheldActivator : MonoBehaviour
{
    [SerializeField] string activatorTag = null;
    [SerializeField] bool deactivateOnExit = false;
    [SerializeField] Cinemachine.CinemachineVirtualCamera vcam;
    public Cinemachine.NoiseSettings handheld;
    public Cinemachine.NoiseSettings normal;
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(activatorTag))
        {
            vcam.AddCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = handheld;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (deactivateOnExit && collision.CompareTag(activatorTag))
        {
            vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = normal;
        }
    }
}
