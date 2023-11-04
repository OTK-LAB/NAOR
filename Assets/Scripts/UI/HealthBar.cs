using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class HealthBar : ProgressBar
{
	private void Start() 
	{
		SetMaxValue(PlayerMain.Instance.PlayerData.healthSystem.MaxHealth);
		SetValue(PlayerMain.Instance.PlayerData.healthSystem.CurrentHealth);
		PlayerMain.Instance.PlayerData.healthSystem.OnMaxHealthChanged += SetMaxValue;
		PlayerMain.Instance.PlayerData.healthSystem.OnHealthChanged += SetValue;
	}
}
