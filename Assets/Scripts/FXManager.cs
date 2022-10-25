using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private    AudioSource[]       WalkSounds;
    [SerializeField] private    AudioSource[]       JumpSounds;
    [Header("")]
    [SerializeField] private    ParticleSystem[]    PS;
    private                     int                 flip = 0;
    private                     bool                doJumpParticle;
    [SerializeField] private    LayerMask           groundLayer;
    public                      Transform           groundCheck;

    void WalkParticle() {
        PS[0].Play();
    }

    void OkeyToJumpParticle() {
        doJumpParticle = true;
    }

    void JumpParticle() {
        PS[1].Play();
    }

    void PlayWalkSound() {
        WalkSounds[flip].Play();
        flip ^= 1;
    }

    void PlayJump() {
        JumpSounds[0].Play();
    }

    void PlayLanding() {
        JumpSounds[1].Play();
    }

    private void Update() {
        if (doJumpParticle && Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer))
        {
            JumpParticle();
            doJumpParticle = false;
        }
    }
}
