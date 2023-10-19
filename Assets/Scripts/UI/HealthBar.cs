using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;

public class HealthBar : ProgressBar
{
	public PlayerMain player;
	private void Start() {
		player = PlayerMain.Instance;
		SetMaxValue(player.playerHealthSystem.MaxHealth);
		SetValue(player.playerHealthSystem.CurrentHealth);
		player.playerHealthSystem.OnMaxHealthChanged += SetMaxValue;
		player.playerHealthSystem.OnHealthChanged += SetValue;
	}
}
