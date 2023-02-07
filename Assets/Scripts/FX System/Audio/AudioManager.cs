using System;
using UnityEngine;

public static class AudioManager
{
    [System.Serializable]
    public class Audio
    {
        public enum SFX
        {
            SFX1,
            SFX2,
            SFX3
        }
        public enum VoiceLine
        {
            Dialogue1_1,
            Dialogue1_2,
            Dialogue2_1
        }
        public enum Soundtrack
        {
            Music1,
            Music2,
            Music3
        }
    }
    public static AudioSource Play(Audio.SFX audio, float volume = 1f, AudioSource audioSource = default, bool isOneShot = default, bool destroyAtFinish = default, Vector3 point = default, Transform parent = default)
    {
        if (Array.Exists<ResourceManager.SFX>(ResourceManager.i.sfx, x => x.clip == audio))
        {
            if (audioSource == null) audioSource = AddObjectWithAudioSource("SFXAudio", parent, point);
            AudioClip audioClip = Array.Find<ResourceManager.SFX>(ResourceManager.i.sfx, x => x.clip == audio).audio;
            if (isOneShot) audioSource.PlayOneShot(audioClip, volume);
            else
            {
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
            }
            if (destroyAtFinish) GameObject.Destroy(audioSource.gameObject, audioClip.length);
            return audioSource;
        }
        Debug.LogError("SFX(" + audio.ToString() + ") not found!");
        return null;
    }
    public static AudioSource Play(Audio.VoiceLine audio, float volume = 1f, AudioSource audioSource = default, bool isOneShot = default, bool destroyAtFinish = default, Vector3 point = default, Transform parent = null)
    {
        if (Array.Exists<ResourceManager.VoiceLine>(ResourceManager.i.voiceLine, x => x.clip == audio))
        {
            if (audioSource == null) audioSource = AddObjectWithAudioSource("VoiceLineAudio", parent, point);
            AudioClip audioClip = Array.Find<ResourceManager.VoiceLine>(ResourceManager.i.voiceLine, x => x.clip == audio).audio;
            if (isOneShot) audioSource.PlayOneShot(audioClip, volume);
            else
            {
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
            }
            if (destroyAtFinish) GameObject.Destroy(audioSource.gameObject, audioClip.length);
            return audioSource;
        }
        Debug.LogError("VoiceLine(" + audio.ToString() + ") not found!");
        return null;
    }
    public static AudioSource Play(Audio.Soundtrack audio, float volume = 1f, AudioSource audioSource = default, bool isOneShot = default, bool destroyAtFinish = default, Vector3 point = default, Transform parent = null)
    {
        if (Array.Exists<ResourceManager.Soundtrack>(ResourceManager.i.soundtrack, x => x.clip == audio))
        {
            if (audioSource == null) audioSource = AddObjectWithAudioSource("SoundtrackAudio", parent, point);
            AudioClip audioClip = Array.Find<ResourceManager.Soundtrack>(ResourceManager.i.soundtrack, x => x.clip == audio).audio;
            if (isOneShot) audioSource.PlayOneShot(audioClip, volume);
            else
            {
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
            }
            if (destroyAtFinish) GameObject.Destroy(audioSource.gameObject, audioClip.length);
            return audioSource;
        }
        Debug.LogError("Soundtrack(" + audio.ToString() + ") not found!");
        return null;
    }
    private static AudioSource AddObjectWithAudioSource(string name, Transform parent, Vector3 point)
    {
        AudioSource audioSource;
        audioSource = new GameObject(name).AddComponent<AudioSource>();
        audioSource.transform.parent = parent;
        audioSource.transform.localPosition = point;
        return audioSource;
    }
}
