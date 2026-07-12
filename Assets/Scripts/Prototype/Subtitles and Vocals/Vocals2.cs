using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals2 : MonoBehaviour
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
        ExitRiverUI.instance.SetSubtitle(clip.subtitle, clip.clip.length);
    }

}