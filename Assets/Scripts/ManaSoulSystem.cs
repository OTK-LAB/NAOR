using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSoulSystem : MonoBehaviour
{
    [SerializeField]
    private float currentMana;
    [SerializeField]
    private float maxMana;

    [SerializeField]
    private float currentSoul;
    [SerializeField]
    private float maxSoul;

    public ProgressBar manaBar;
    public ProgressBar soulBar;
    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        manaBar.SetMaxValue(maxMana);

        currentSoul = 0;
        soulBar.SetMaxValue(maxSoul);
        soulBar.SetValue(currentSoul);
    }

    public void UseMana(float manaAmount)
    {
        if (currentMana < manaAmount) // Önce mana barý bitiriliyor sonra soul barý harcanýyor
        {
            if (currentSoul + currentMana >= manaAmount)
            {
                manaAmount -= currentMana;
                currentMana = 0;
                currentSoul -= manaAmount;
                soulBar.SetValue(currentSoul);
            }
        }
        else
            currentMana -= manaAmount;
        manaBar.SetValue(currentMana);
    }

    public void AddSoul (float soulAmount)
    {
        currentSoul += soulAmount;
        if (currentSoul > maxSoul)
            currentSoul = maxSoul;
        soulBar.SetValue(currentSoul);
    }

    public void UseSoul (float soulAmount)
    {
        if (soulAmount <= currentSoul)
            currentSoul -= soulAmount;
        soulBar.SetValue(currentSoul);
    }
}
