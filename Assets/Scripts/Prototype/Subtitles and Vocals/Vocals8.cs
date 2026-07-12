using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals8 : MonoBehaviour
{
    private static Vocals8 _instance;
    public static Vocals8 instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<Vocals8>();
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
        AfterRiver.instance.SetSubtitle(clip.subtitle, clip.clip.length);
    }

}