using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioSystem.Audio;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class EventFunctions : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<RuleSFX> ruleSFX;
    [SerializeField] private List<RuleVoiceLine> ruleVoiceLine;
    [SerializeField] private List<RuleSoundtrack> ruleSoundtrack;

    [Header("VFX")]
    [SerializeField] private List<RuleParticle> ruleParticle;
    [SerializeField] private List<RuleVFXGraph> ruleVFXGraph;

    #region Audio Classes
    public class RuleAudio<T>
    {
        public enum Mode { Single, Queue, Random }

        public T clip;
        [NonEditable] public int currentIndex = 0;
        public Mode playMode = Mode.Single;
        public MinMaxCurve volume = 1f;
        public MinMaxCurve pitch = 1f;
        public AudioSource audioSource = null;
        [System.Serializable]
        public class TransformVariable
        {
            public Transform parent = null;
            public Vector3 parentPosition = Vector3.zero;
        }
        [System.Serializable]
        public class ExtraVariable
        {
            public bool isOneShot = false;
            public bool destroyAtFinish = false;
            public bool keepNewAudioSource = false;
        }
        public TransformVariable transform;
        public ExtraVariable extra;
    }
    [System.Serializable]
    public class RuleSFX : RuleAudio<SFX[]>
    {
        public RuleSFX(SFX clip)
        {
            this.clip = new SFX[1];
            this.clip[0] = clip;
        }
    }

    [System.Serializable]
    public class RuleVoiceLine : RuleAudio<VoiceLine[]>
    {
        public RuleVoiceLine(VoiceLine clip)
        {
            this.clip = new VoiceLine[1];
            this.clip[0] = clip;
        }
    }

    [System.Serializable]
    public class RuleSoundtrack : RuleAudio<Soundtrack[]>
    {
        public RuleSoundtrack(Soundtrack clip)
        {
            this.clip = new Soundtrack[1];
            this.clip[0] = clip;
        }
    }
    #endregion
    #region Audio Functions
    private RuleSFX FindValues(SFX name) { return ruleSFX.Any(x => x.clip[0] == name) ? ruleSFX.Find(x => x.clip[0] == name) : new RuleSFX(name); }
    private RuleVoiceLine FindValues(VoiceLine name) { return ruleVoiceLine.Any(x => x.clip[0] == name) ? ruleVoiceLine.Find(x => x.clip[0] == name) : new RuleVoiceLine(name); }
    private RuleSoundtrack FindValues(Soundtrack name) { return ruleSoundtrack.Any(x => x.clip[0] == name) ? ruleSoundtrack.Find(x => x.clip[0] == name) : new RuleSoundtrack(name); }

    public void PlaySFX(SFX audio)
    {
        RuleSFX sfx = FindValues(audio);
        float volume = sfx.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sfx.volume.constantMin, sfx.volume.constantMax) : sfx.volume.Evaluate(Random.Range(0f, 1f));
        float pitch = sfx.pitch.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sfx.pitch.constantMin, sfx.pitch.constantMax) : sfx.pitch.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioSystem.Play(sfx.clip[sfx.currentIndex], volume, pitch, sfx.audioSource, sfx.extra.isOneShot, sfx.extra.destroyAtFinish, sfx.transform.parentPosition, sfx.transform.parent);
        sfx.audioSource = sfx.extra.keepNewAudioSource ? temp : sfx.audioSource;
        if (sfx.playMode == RuleSFX.Mode.Single) sfx.currentIndex = 0;
        else if (sfx.playMode == RuleSFX.Mode.Queue) _ = sfx.currentIndex < sfx.clip.Length - 1 ? sfx.currentIndex++ : sfx.currentIndex = 0;
        else if (sfx.playMode == RuleSFX.Mode.Random) sfx.currentIndex = Random.Range(0, sfx.clip.Length);
    }
    public void PlayVoiceLine(VoiceLine audio)
    {
        RuleVoiceLine vLine = FindValues(audio);
        float volume = vLine.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(vLine.volume.constantMin, vLine.volume.constantMax) : vLine.volume.Evaluate(Random.Range(0f, 1f));
        float pitch = vLine.pitch.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(vLine.pitch.constantMin, vLine.pitch.constantMax) : vLine.pitch.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioSystem.Play(vLine.clip[vLine.currentIndex], volume, pitch, vLine.audioSource, vLine.extra.isOneShot, vLine.extra.destroyAtFinish, vLine.transform.parentPosition, vLine.transform.parent);
        vLine.audioSource = vLine.extra.keepNewAudioSource ? temp : vLine.audioSource;
        if (vLine.playMode == RuleVoiceLine.Mode.Single) vLine.currentIndex = 0;
        else if (vLine.playMode == RuleVoiceLine.Mode.Queue) _ = vLine.currentIndex < vLine.clip.Length - 1 ? vLine.currentIndex++ : vLine.currentIndex = 0;
        else if (vLine.playMode == RuleVoiceLine.Mode.Random) vLine.currentIndex = Random.Range(0, vLine.clip.Length);
    }
    public void PlaySoundtrack(Soundtrack audio)
    {
        RuleSoundtrack sTrack = FindValues(audio);
        float volume = sTrack.volume.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sTrack.volume.constantMin, sTrack.volume.constantMax) : sTrack.volume.Evaluate(Random.Range(0f, 1f));
        float pitch = sTrack.pitch.mode == ParticleSystemCurveMode.TwoConstants ? Random.Range(sTrack.pitch.constantMin, sTrack.pitch.constantMax) : sTrack.pitch.Evaluate(Random.Range(0f, 1f));
        AudioSource temp = AudioSystem.Play(sTrack.clip[sTrack.currentIndex], volume, pitch, sTrack.audioSource, sTrack.extra.isOneShot, sTrack.extra.destroyAtFinish, sTrack.transform.parentPosition, sTrack.transform.parent);
        sTrack.audioSource = sTrack.extra.keepNewAudioSource ? temp : sTrack.audioSource;
        if (sTrack.playMode == RuleSoundtrack.Mode.Single) sTrack.currentIndex = 0;
        else if (sTrack.playMode == RuleSoundtrack.Mode.Queue) _ = sTrack.currentIndex < sTrack.clip.Length - 1 ? sTrack.currentIndex++ : sTrack.currentIndex = 0;
        else if (sTrack.playMode == RuleSoundtrack.Mode.Random) sTrack.currentIndex = Random.Range(0, sTrack.clip.Length);
    }
    #endregion

    #region VFX Classes
    public class RuleVFX<T>
    {
        public T name;
        [NonEditable] public GameObject gameObject = null;
        public Quaternion rotation = Quaternion.identity;
        public Transform parent = null;
        public Vector3 localPosition = Vector3.zero;
    }
    [System.Serializable]
    public class RuleParticle : RuleVFX<VFXSystem.VFX.ParticleSystem>
    {
        public bool clearBeforePlay = false;
        public bool destroyAtFinish = false;
    }
    [System.Serializable]
    public class RuleVFXGraph : RuleVFX<VFXSystem.VFX.VFXGraph>
    {
        public bool reinitializeBeforePlay = false;
    }
    #endregion
    #region VFX Functions
    private RuleParticle FindValues(VFXSystem.VFX.ParticleSystem name) { return ruleParticle.Any(x => x.name == name) ? ruleParticle.Find(x => x.name == name) : new RuleParticle(); }
    private RuleVFXGraph FindValues(VFXSystem.VFX.VFXGraph name) { return ruleVFXGraph.Any(x => x.name == name) ? ruleVFXGraph.Find(x => x.name == name) : new RuleVFXGraph();
}

public void PlayParticle(VFXSystem.VFX.ParticleSystem vfx)
    {
        RuleParticle particle = FindValues(vfx);
        particle.gameObject = VFXSystem.Play(vfx, particle.gameObject, particle.localPosition, particle.rotation, particle.parent, particle.clearBeforePlay, particle.destroyAtFinish);
    }

    public void PlayVFXGraph(VFXSystem.VFX.VFXGraph vfx)
    {
        RuleVFXGraph vfxGraph = FindValues(vfx);
        vfxGraph.gameObject = VFXSystem.Play(vfx, vfxGraph.gameObject, vfxGraph.localPosition, vfxGraph.rotation, vfxGraph.parent, vfxGraph.reinitializeBeforePlay);
    }
    #endregion
}
