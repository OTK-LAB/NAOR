using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/HealthGateList")]
public class HealthGateListSO : ScriptableObject
{
    public List<HealthGateSO> HealthGateList;
}
