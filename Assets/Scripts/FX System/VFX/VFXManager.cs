using UnityEngine;
using System;
using UnityEngine.VFX;

public static class VFXManager
{
    [System.Serializable]
    public class VFX
    {
        public enum ParticleSystem
        {
            ParticleSystem1,
            ParticleSystem2,
            ParticleSystem3
        }
        public enum VFXGraph
        {
            VFXGraph1,
            VFXGraph2,
            VFXGraph3
        }
    }

    public static GameObject Play(VFX.ParticleSystem vfx, GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, bool clearBeforePlay = false, bool destroyAtFinish = false)
    {
        if (Array.Exists<ResourceManager.Particle>(ResourceManager.i.particle, x => x.name == vfx))
        {
            if (gameObject == null) gameObject = GameObject.Instantiate(Array.Find<ResourceManager.Particle>(ResourceManager.i.particle, x => x.name == vfx).prefab, position, rotation, parent);
            ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
            if (clearBeforePlay) particleSystem.Clear();
            particleSystem.Play();
            if (destroyAtFinish) GameObject.Destroy(gameObject, particleSystem.main.duration);
            return gameObject;
        }
        Debug.Log("Particle Prefab" + vfx.ToString() + " not found!");
        return null;
    }

    public static GameObject Play(VFX.VFXGraph vfx, GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, bool clearBeforePlay = false)
    {
        if (Array.Exists<ResourceManager.VFXGraph>(ResourceManager.i.vfxGraph, x => x.name == vfx))
        {
            if (gameObject == null) gameObject = GameObject.Instantiate(Array.Find<ResourceManager.VFXGraph>(ResourceManager.i.vfxGraph, x => x.name == vfx).prefab, position, rotation, parent);
            VisualEffect visualEffect = gameObject.GetComponent<VisualEffect>();

            if (clearBeforePlay) visualEffect.Reinit();
            else visualEffect.Play();

            return gameObject;
        }
        Debug.Log("VFXGraph Prefab" + vfx.ToString() + " not found!");
        return null;
    }
}
