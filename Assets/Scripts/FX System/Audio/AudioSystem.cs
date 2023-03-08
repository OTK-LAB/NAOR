using System;
using UnityEngine;

public static class AudioSystem
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

    /// <summary>
    /// Plays Audioclips which are restored in ResourceManger and type of AudioManager.Audio.SFX, AudioManager.Audio.VoiceLine, AudioManager.Audio.Soundtrack
    /// </summary>
    /// <param audio="audio2"></param>
    /// <returns></returns>
    public static AudioSource Play<T>(T audio, float volume = 1f, float pitch = 1f, AudioSource audioSource = default, bool isOneShot = default, bool destroyAtFinish = default, Vector3 point = default, Transform parent = default) where T : struct, IConvertible
    {
        string name = "";
        ResourceManager.AudioClips<T>[] clips = null;
        if (typeof(T) == typeof(Audio.SFX))
        {
            name = "SFXAudio";
            clips = ResourceManager.i.sfx as ResourceManager.AudioClips<T>[];
        }
        else if (typeof(T) == typeof(Audio.VoiceLine))
        {
            name = "VoiceLineAudio";
            clips = ResourceManager.i.voiceLine as ResourceManager.AudioClips<T>[];
        }
        else if (typeof(T) == typeof(Audio.Soundtrack))
        {
            name = "SoundtrackAudio";
            clips = ResourceManager.i.soundtrack as ResourceManager.AudioClips<T>[];
        }
        else
        {
            Debug.LogError("Invalid audio type: " + typeof(T).ToString());
            return null;
        }

        if (Array.Exists<ResourceManager.AudioClips<T>>(clips, x => x.clip.Equals(audio)))
        {
            if (audioSource == null) audioSource = AddObjectWithAudioSource(name, parent, point);
            audioSource.pitch = pitch;
            AudioClip audioClip = Array.Find<ResourceManager.AudioClips<T>>(clips, x => x.clip.Equals(audio)).audio;
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
        Debug.LogError(typeof(T).ToString() + "(" + audio.ToString() + ") not found!");
        return null;
    }

    private static AudioSource AddObjectWithAudioSource(string name, Transform parent, Vector3 point)
    {
        AudioSource audioSource = new GameObject(name).AddComponent<AudioSource>();
        audioSource.transform.parent = parent;
        audioSource.transform.localPosition = point;
        return audioSource;
    }
}
