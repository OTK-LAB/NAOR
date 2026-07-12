using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals3 : MonoBehaviour
{
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
        PlatformUI.instance.SetSubtitle(clip.subtitle, clip.clip.length);
    }

}