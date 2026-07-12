using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals5 : MonoBehaviour
{
    private static Vocals5 _instance;
    public static Vocals5 instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<Vocals5>();
            return _instance;
        }
    }

    private AudioSource source;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }
    public void Say(AudioObject clip)
    {
        if (source.isPlaying)
            source.Stop();
        source.PlayOneShot(clip.clip);
        BossUI.instance.SetSubtitle(clip.subtitle, clip.clip.length);
    }

}