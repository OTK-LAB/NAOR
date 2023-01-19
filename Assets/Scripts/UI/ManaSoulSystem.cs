using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSoulSystem : MonoBehaviour
{
    [SerializeField]
    public float currentMana;
    [SerializeField]
    public float regenMana;
    [SerializeField]
    public float maxMana;

    [SerializeField]
    private float currentSoul;
    [SerializeField]
    public float maxSoul;

    public ProgressBar manaBar;
    public ProgressBar soulBar;

    public HealthSystem healthSystem;

    private float elapsed = 0f;
    private float regenDelay = 1.0f;
    public float smoothing = 5;

    void Start()
    { 
        currentMana = maxMana;
        manaBar.SetMaxValue(maxMana);

        currentSoul = 0;
        soulBar.SetMaxValue(maxSoul);
        soulBar.SetValue(currentSoul);
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
        
        if (currentMana != manaBar.slider.value)
            manaBar.SetValue(Mathf.Lerp(manaBar.slider.value, currentMana, smoothing * Time.deltaTime));
        if (currentSoul != soulBar.slider.value)
            soulBar.SetValue(Mathf.Lerp(soulBar.slider.value, currentSoul, smoothing * Time.deltaTime));

    }
    IEnumerator Wait(float second)
    {
        yield return new WaitForSeconds(second);
    }

    public void UseMana(float manaAmount)
    {
        smoothing = 10;
        if (currentMana < manaAmount) // Önce mana barý bitiriliyor sonra soul barý harcanýyor
        {
            if (currentSoul + currentMana >= manaAmount)
            {
                manaAmount -= currentMana;
                currentMana = 0;
                currentSoul -= manaAmount;
               // soulBar.SetValue(currentSoul);
            }
        }
        else
            currentMana -= manaAmount;
        //manaBar.SetValue(currentMana);
    }

    public void AddMana(float manaAmount)
    {
        smoothing = 5;
        currentMana += manaAmount;
        if (currentMana > maxMana)
            currentMana = maxMana;
        //manaBar.SetValue(currentMana);
    }

    public void AddSoul (float soulAmount)
    {
        smoothing = 5;
        currentSoul += soulAmount;
        if (currentSoul > maxSoul)
            currentSoul = maxSoul;
        //soulBar.SetValue(currentSoul);
    }

    public void UseSoul (float soulAmount)
    {
        smoothing = 10;
        if (soulAmount <= currentSoul)
            currentSoul -= soulAmount;

       // soulBar.SetValue(currentSoul);  
    }

    public void HealWithSoul(float healAmount)
    {
        smoothing = 5;
        if (currentSoul >= healAmount && healthSystem.currentHealth < healthSystem.maxHealth)
        {
            healthSystem.Heal(healAmount);
            UseSoul(healAmount);
        }
    }
}
