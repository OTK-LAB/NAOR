using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals7 : MonoBehaviour
{
    private AudioSource source;
    public static Vocals7 instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
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
        AfterFall.instance.SetSubtitle(clip.subtitle, clip.clip.length);
    }

}