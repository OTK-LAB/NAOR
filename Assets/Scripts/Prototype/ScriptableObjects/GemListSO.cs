using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GemList")]
public class GemListSO : ScriptableObject
{
    public List<GemSO> gemList;
}
