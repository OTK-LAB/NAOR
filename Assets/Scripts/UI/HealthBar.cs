using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class HealthBar : ProgressBar
{
	public PlayerMain player;
	private void Start() {
		player = PlayerMain.Instance;
		SetMaxValue(player.PlayerData.healthSystem.MaxHealth);
		SetValue(player.PlayerData.healthSystem.CurrentHealth);
		player.PlayerData.healthSystem.OnMaxHealthChanged += SetMaxValue;
		player.PlayerData.healthSystem.OnHealthChanged += SetValue;
	}
}
