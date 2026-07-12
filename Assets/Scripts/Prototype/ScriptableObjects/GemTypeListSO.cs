using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GemTypeList")]
public class GemTypeListSO : ScriptableObject
{
    public List<GemListSO> gemTypeList;
}
