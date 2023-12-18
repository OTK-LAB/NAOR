using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UltimateCC;
using UnityEngine;
[Serializable]
public class HealthSystem
{
    private bool invincible = false;
    [SerializeField] private float currentHealth = 1000;
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float damageMultiplier = 1;
    //float smoothing = 5;

    public delegate void HealthHandler(float amount);
    public delegate void DeathHandler();
    public event HealthHandler OnHealthChanged;
    public event HealthHandler OnMaxHealthChanged;
    public event DeathHandler OnDied;
    public bool Invincible { set { invincible = value; } }
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(value);
        }
    }
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
            OnMaxHealthChanged?.Invoke(value);
        }
    }

    public float DamageMultiplier { get { return damageMultiplier; } set { damageMultiplier = value; } }

    public void Damage(float damageAmount)
    {
        PlayerMain.Instance.Animator.SetTrigger("Hurt");
        ShakeCamera(2f, 2f, 0.08f, 0.04f);
        if (!invincible)
        {
            currentHealth -= damageAmount * damageMultiplier;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDied?.Invoke();
            }
            OnHealthChanged?.Invoke(currentHealth);
        }
        //healthBar.SetValue(currentHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth);
        //healthBar.SetValue(currentHealth);
    }

    public void ShakeCamera(float frequency, float amplitude, float duration1, float duration2)
    {
        var activeVirtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;

        if (activeVirtualCamera is CinemachineVirtualCamera virtualCamera)
        {
            var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = amplitude;
            DOTween.To(() => noise.m_FrequencyGain, x => noise.m_FrequencyGain = x, frequency, duration1)
                .OnComplete(() =>
                {
                    DOTween.To(() => noise.m_FrequencyGain, x => noise.m_FrequencyGain = x, 0f, duration2).OnComplete(() =>
                    {
                        noise.m_AmplitudeGain = 0;
                    });
                });
        }
        else
        {
            Debug.LogWarning("Active virtual camera is not a CinemachineVirtualCamera.");
        }
    }
}