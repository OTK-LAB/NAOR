using UnityEngine;
using static ResourceManager;

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
    public class AudioClips<T>
    {
        public T clip;
        public AudioClip audio;
    }
    #endregion
    [Header("Audio")]
    public AudioClips<AudioSystem.Audio.SFX>[] sfx;
    public AudioClips<AudioSystem.Audio.VoiceLine>[] voiceLine;
    public AudioClips<AudioSystem.Audio.Soundtrack>[] soundtrack;

    #region VfX Classes
    [System.Serializable]
    public class VFXPrefabs<T>
    {
        public T name;
        public GameObject prefab;
    }
    #endregion
    [Header("Particle System")]
    public VFXPrefabs<VFXSystem.VFX.ParticleSystem>[] particle;
    public VFXPrefabs<VFXSystem.VFX.VFXGraph>[] vfxGraph;

}
