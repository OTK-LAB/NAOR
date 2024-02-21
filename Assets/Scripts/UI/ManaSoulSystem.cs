using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class ManaSoulSystem : MonoBehaviour
{
    [SerializeField]
    private float currentMana;
    [SerializeField]
    public float regenMana;
    [SerializeField]
    private float maxMana;
    public float CurrentMana { get { return currentMana; } set { currentMana = value; } }
    public float RegenMana { get { return regenMana; } set { regenMana = value; } }

    public float MaxMana
    {
        get
        {
            return maxMana;
        }
        set
        {
            maxMana = value;
            OnMaxManaChanged?.Invoke(value);
        }
    }
    [SerializeField]
    public float currentSoul;
    [SerializeField]
    public float maxSoul;
    
    public delegate void ValueHandler(float value);
    public static event ValueHandler OnManaChanged;
    public static event ValueHandler OnMaxManaChanged;
    public static event ValueHandler OnSoulChanged;
    public static event ValueHandler OnMaxSoulChanged;

    private PlayerMain player;

    private float elapsed = 0f;
    private float regenDelay = 1.0f;
    public float smoothing = 5;

    void Start()
    { 
        player = PlayerMain.Instance;
        OnMaxManaChanged?.Invoke(maxMana);
        OnManaChanged?.Invoke(maxMana);

        currentSoul = 0;
        OnMaxSoulChanged?.Invoke(maxSoul);
        OnSoulChanged?.Invoke(currentSoul);
    }

    void Update()
    {
        
        if (elapsed > regenDelay && currentMana < maxMana)
        {
            AddMana(regenMana);
            elapsed = 0f;
        }
        elapsed += Time.deltaTime;
        //StartCoroutine(Wait(1));
    }
    IEnumerator Wait(float second)
    {
        yield return new WaitForSeconds(second);
    }

    public void UseMana(float manaAmount)
    {
        smoothing = 10;
        if (currentMana < manaAmount) // �nce mana bar� bitiriliyor sonra soul bar� harcan�yor
        {
            if (currentSoul + currentMana >= manaAmount)
            {
                manaAmount -= currentMana;
                currentMana = 0;
                currentSoul -= manaAmount;
                OnSoulChanged?.Invoke(currentSoul);
            }
        }
        else
            currentMana -= manaAmount;
        //manaBar.SetValue(currentMana);
        OnManaChanged?.Invoke(currentMana);
    }

    public void AddMana(float manaAmount)
    {
        smoothing = 5;
        currentMana += manaAmount;
        if (currentMana > maxMana)
            currentMana = maxMana;
        //manaBar.SetValue(currentMana);
        OnManaChanged?.Invoke(currentMana);
    }

    public void AddSoul (float soulAmount)
    {
        smoothing = 5;
        currentSoul += soulAmount;
        if (currentSoul > maxSoul)
            currentSoul = maxSoul;
        //soulBar.SetValue(currentSoul);
        OnSoulChanged?.Invoke(currentSoul);
    }

    public void UseSoul (float soulAmount)
    {
        smoothing = 10;
        if (soulAmount <= currentSoul)
            currentSoul -= soulAmount;
       // soulBar.SetValue(currentSoul);
        OnSoulChanged?.Invoke(currentSoul);  
    }

    public void HealWithSoul(float healAmount)
    {
        smoothing = 5;
        if (currentSoul >= healAmount)
        {
            player.PlayerData.healthSystem.Heal(healAmount);
            UseSoul(healAmount);
        }
    }
}
