using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private    AudioSource[]       WalkSounds;
    [SerializeField] private    AudioSource[]       JumpSounds;
    [SerializeField] private    AudioSource[]       SwordSounds;
    [Header("")]
    [SerializeField] private    ParticleSystem[]    PS;
    private                     int                 flip = 0;
    private                     bool                doJumpParticle;
    [SerializeField] private    LayerMask           groundLayer;
    public                      Transform           groundCheck;
    private                     int                 groundType;

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
        WalkSounds[groundType + flip].Play();
        flip ^= 1;
    }

    void PlayJump() {
        JumpSounds[0].Play();
    }

    void PlayLanding() {
        JumpSounds[1].Play();
    }

    void PlaySwordSwing() {
        SwordSounds[0].Play();
    }

    void PlayHeavyCharge()
    {
        SwordSounds[1].Play();
    }

    void PlayHeavyAttack()
    {
        SwordSounds[2].Play();
    }

    void PlayChargePS()
    {
        PS[3].Play();
    }

    void PlayHeavyPS()
    {
        PS[4].Play();
    }

    private void Update() {
        if (doJumpParticle && Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer))
        {
            JumpParticle();
            doJumpParticle = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground_1") {
            groundType = 0;
        }

        else if (collision.gameObject.tag == "Ground_2") {
            groundType = 2;
        }

        else {
            groundType = 0;
        }
    }
}
