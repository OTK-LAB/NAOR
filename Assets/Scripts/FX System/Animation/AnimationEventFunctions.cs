using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class AnimationEventFunctions : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private RuleSFX[] ruleSFX = null;
    [SerializeField] private RuleVoiceLine[] ruleVoiceLine = null;
    [SerializeField] private RuleSoundtrack[] ruleSoundtrack = null;

    [Header("VFX")]
    [SerializeField] private RuleParticle[] ruleParticle = null;
    [SerializeField] private RuleVFXGraph[] ruleVFXGraph = null;

    #region Audio Classes
    [System.Serializable]
    public class RuleSFX
    {
        public enum Mode { Single, Queue, Random }

        public AudioManager.Audio.SFX[] clip = new AudioManager.Audio.SFX[1];
        [ReadOnly] public int currentIndex;
        public Mode playMode;
        public MinMaxCurve volume;
        public AudioSource audioSource;
        public Transform parent;
        public Vector3 point;
        public bool isOneShot;
        public bool destroyAtFinish;
        public bool keepNewAudioSource;
        public RuleSFX(AudioManager.Audio.SFX clip)
        {
            this.clip[0] = clip;
            this.currentIndex = 0;
            this.playMode = Mode.Single;
            this.volume = 1f;
            this.audioSource = null;
            this.parent = null;
            this.point = Vector3.zero;
            this.isOneShot = false;
            this.destroyAtFinish = false;
            this.keepNewAudioSource = false;
        }
    }

    [System.Serializable]
    public class RuleVoiceLine
    {
        public enum Mode { Single, Queue, Random }

        public AudioManager.Audio.VoiceLine[] clip = new AudioManager.Audio.VoiceLine[1];
        [ReadOnly] public int currentIndex;
        public Mode playMode;
        public MinMaxCurve volume;
        public AudioSource audioSource;
        public Transform parent;
        public Vector3 point;
        public bool isOneShot;
        public bool destroyAtFinish;
        public bool keepNewAudioSource;
        public RuleVoiceLine(AudioManager.Audio.VoiceLine clip)
        {
            this.clip[0] = clip;
            this.currentIndex = 0;
            this.playMode = Mode.Single;
            this.volume = 1f;
            this.audioSource = null;
            this.parent = null;
            this.point = Vector3.zero;
            this.isOneShot = false;
            this.destroyAtFinish = false;
            this.keepNewAudioSource = false;
        }
    }

    [System.Serializable]
    public class RuleSoundtrack
    {
        public enum Mode { Single, Queue, Random }

        public AudioManager.Audio.Soundtrack[] clip = new AudioManager.Audio.Soundtrack[1];
        [ReadOnly] public int currentIndex;
        public Mode playMode = Mode.Single;
        public MinMaxCurve volume;
        public AudioSource audioSource;
        public Transform parent;
        public Vector3 point;
        public bool isOneShot;
        public bool destroyAtFinish;
        public bool keepNewAudioSource;
        public RuleSoundtrack(AudioManager.Audio.Soundtrack clip)
        {
            this.clip[0] = clip;
            this.currentIndex = 0;
            this.playMode = Mode.Single;
            this.volume = 1f;
            this.audioSource = null;
            this.parent = null;
            this.point = Vector3.zero;
            this.isOneShot = false;
            this.destroyAtFinish = false;
            this.keepNewAudioSource = false;
        }
    }
    #endregion
    #region Audio Functions
    private RuleSFX FindValues(AudioManager.Audio.SFX name) { return Array.Exists<RuleSFX>(ruleSFX, x => x.clip[0] == name) ? Array.Find<RuleSFX>(ruleSFX, x => x.clip[0] == name) : new RuleSFX(name); }
    private RuleVoiceLine FindValues(AudioManager.Audio.VoiceLine name) { return Array.Exists<RuleVoiceLine>(ruleVoiceLine, x => x.clip[0] == name) ? Array.Find<RuleVoiceLine>(ruleVoiceLine, x => x.clip[0] == name) : new RuleVoiceLine(name); }
    private RuleSoundtrack FindValues(AudioManager.Audio.Soundtrack name) { return Array.Exists<RuleSoundtrack>(ruleSoundtrack, x => x.clip[0] == name) ? Array.Find<RuleSoundtrack>(ruleSoundtrack, x => x.clip[0] == name) : new RuleSoundtrack(name); }

    public void PlaySFX(AudioManager.Audio.SFX audio)
    {
        RuleSFX sfx = FindValues(audio);
        float volume = sfx.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sfx.volume.constantMin, sfx.volume.constantMax) : sfx.volume.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioManager.Play(sfx.clip[sfx.currentIndex], volume, sfx.audioSource, sfx.isOneShot, sfx.destroyAtFinish, sfx.point, sfx.parent);
        sfx.audioSource = sfx.keepNewAudioSource ? temp : sfx.audioSource;
        if (sfx.playMode == RuleSFX.Mode.Single) sfx.currentIndex = 0;
        else if (sfx.playMode == RuleSFX.Mode.Queue) _ = sfx.currentIndex < sfx.clip.Length - 1 ? sfx.currentIndex++ : sfx.currentIndex = 0;
        else if (sfx.playMode == RuleSFX.Mode.Random) sfx.currentIndex = Random.Range(0, sfx.clip.Length);
    }
    public void PlayVoiceLine(AudioManager.Audio.VoiceLine audio)
    {
        RuleVoiceLine vLine = FindValues(audio);
        float volume = vLine.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(vLine.volume.constantMin, vLine.volume.constantMax) : vLine.volume.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioManager.Play(vLine.clip[vLine.currentIndex], volume, vLine.audioSource, vLine.isOneShot, vLine.destroyAtFinish, vLine.point, vLine.parent);
        vLine.audioSource = vLine.keepNewAudioSource ? temp : vLine.audioSource;
        if (vLine.playMode == RuleVoiceLine.Mode.Single) vLine.currentIndex = 0;
        else if (vLine.playMode == RuleVoiceLine.Mode.Queue) _ = vLine.currentIndex < vLine.clip.Length - 1 ? vLine.currentIndex++ : vLine.currentIndex = 0;
        else if (vLine.playMode == RuleVoiceLine.Mode.Random) vLine.currentIndex = Random.Range(0, vLine.clip.Length);
    }
    public void PlaySoundtrack(AudioManager.Audio.Soundtrack audio)
    {
        RuleSoundtrack sTrack = FindValues(audio);
        float volume = sTrack.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sTrack.volume.constantMin, sTrack.volume.constantMax) : sTrack.volume.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioManager.Play(sTrack.clip[sTrack.currentIndex], volume, sTrack.audioSource, sTrack.isOneShot, sTrack.destroyAtFinish, sTrack.point, sTrack.parent);
        sTrack.audioSource = sTrack.keepNewAudioSource ? temp : sTrack.audioSource;
        if (sTrack.playMode == RuleSoundtrack.Mode.Single) sTrack.currentIndex = 0;
        else if (sTrack.playMode == RuleSoundtrack.Mode.Queue) _ = sTrack.currentIndex < sTrack.clip.Length - 1 ? sTrack.currentIndex++ : sTrack.currentIndex = 0;
        else if (sTrack.playMode == RuleSoundtrack.Mode.Random) sTrack.currentIndex = Random.Range(0, sTrack.clip.Length);
    }
    #endregion

    #region VFX Classes
    [System.Serializable]
    public class RuleParticle
    {
        public VFXManager.VFX.ParticleSystem name;
        [ReadOnly] public GameObject gameObject = null;
        public Vector3 localPosition = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Transform parent = null;
        public bool clearBeforePlay = false;
        public bool destroyAtFinish = false;
    }
    [System.Serializable]
    public class RuleVFXGraph
    {
        public VFXManager.VFX.VFXGraph name;
        [ReadOnly] public GameObject gameObject = null;
        public Vector3 localPosition = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Transform parent = null;
        public bool reinitializeBeforePlay = false;
    }
    #endregion
    #region VFX Functions
    private RuleParticle FindValues(VFXManager.VFX.ParticleSystem name) { return Array.Exists<RuleParticle>(ruleParticle, x => x.name == name) ? Array.Find<RuleParticle>(ruleParticle, x => x.name == name) : new RuleParticle(); }
    private RuleVFXGraph FindValues(VFXManager.VFX.VFXGraph name) { return Array.Exists<RuleVFXGraph>(ruleVFXGraph, x => x.name == name) ? Array.Find<RuleVFXGraph>(ruleVFXGraph, x => x.name == name) : new RuleVFXGraph(); }

    public void PlayParticle(VFXManager.VFX.ParticleSystem vfx)
    {
        RuleParticle particle = FindValues(vfx);
        particle.gameObject = VFXManager.Play(vfx, particle.gameObject, particle.localPosition, particle.rotation, particle.parent, particle.clearBeforePlay, particle.destroyAtFinish);
    }

    public void PlayVFXGraph(VFXManager.VFX.VFXGraph vfx)
    {
        RuleVFXGraph vfxGraph = FindValues(vfx);
        vfxGraph.gameObject = VFXManager.Play(vfx, vfxGraph.gameObject, vfxGraph.localPosition, vfxGraph.rotation, vfxGraph.parent, vfxGraph.reinitializeBeforePlay);
    }
    #endregion

}
