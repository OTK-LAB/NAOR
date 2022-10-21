using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField]private ParticleSystem[] PS;
    [SerializeField]private AudioSource WalkSound;

    void WalkParticle() {
        PS[0].Play();
    }
}
