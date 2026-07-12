using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/HealthGate")]
public class HealthGateSO : ScriptableObject
{
    public int percentage;
    public enum Classes
    {
        Legendary,
        Rare,
        Common
    };

    public Classes classes;
    public int maxGemCount;
    public List<GemSO> activeGems;
    public bool isActive;
}
