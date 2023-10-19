using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBar : ProgressBar
{
	private void Start() {
		ManaSoulSystem.OnMaxSoulChanged += SetMaxValue;
		ManaSoulSystem.OnSoulChanged += SetValue;
	}
}
