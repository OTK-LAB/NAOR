using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _i;

    public static ResourceManager i
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("ResourceManager")) as GameObject).GetComponent<ResourceManager>();
                _i.gameObject.name = "ResourceManager";
            }
            return _i;
        }
    }
    #region Audio Classes
    [System.Serializable]
    public class SFX
    {
        public AudioManager.Audio.SFX clip;
        public AudioClip audio;
    }
    [System.Serializable]
    public class VoiceLine
    {
        public AudioManager.Audio.VoiceLine clip;
        public AudioClip audio;
    }
    [System.Serializable]
    public class Soundtrack
    {
        public AudioManager.Audio.Soundtrack clip;
        public AudioClip audio;
    }
    #endregion
    [Header("Audio")]
    public SFX[] sfx;
    public VoiceLine[] voiceLine;
    public Soundtrack[] soundtrack;

    #region VfX Classes
    [System.Serializable]
    public class Particle
    {
        public VFXManager.VFX.ParticleSystem name;
        public GameObject prefab;
    }
    [System.Serializable]
    public class VFXGraph
    {
        public VFXManager.VFX.VFXGraph name;
        public GameObject prefab;
    }
    #endregion
    [Header("Particle System")]
    public Particle[] particle;
    public VFXGraph[] vfxGraph;

}
