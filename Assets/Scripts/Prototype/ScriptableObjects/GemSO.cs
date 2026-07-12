using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Gem")]
public class GemSO : ScriptableObject
{
    public enum Gemtype
    {
        AttackBuff,
        SpeedBuff,
        DefenseBuff,
        ExperienceBuff,
        PowerfulLaunch,
        QuickHand,
        Ghost,
        ShieldUp,
        Vaulter,
        BarbedArmor,
        Regen,
        Acrobatics,
        Runner,
        DurableBody,
        BloodThirsty,
        DexterityIncrease
    };

    public Gemtype gemtype;
    public bool isActive;
    public float buffRate;
    public string title;
    public string description;
}
