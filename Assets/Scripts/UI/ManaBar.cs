using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : ProgressBar
{
	private void Start() {
		ManaSoulSystem.OnMaxManaChanged += SetMaxValue;
		ManaSoulSystem.OnManaChanged += SetValue;
	}
}
