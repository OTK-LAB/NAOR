using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ManaBarWorkAroundOne : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;
	public float easingDuration = 0.5f;
	private void Awake() {
		ManaSoulSystem.OnMaxManaChanged += SetMaxValue;
		ManaSoulSystem.OnManaChanged += SetValue;
	}
	public void SetMaxValue(float value)
	{
		slider.maxValue = value/2;
		slider.value = value/2;

		fill.color = gradient.Evaluate(1f);
	}
	public void SetValue(float value)
	{
		if(value <= slider.maxValue)
		{
			DOTween.To(() => slider.value, x => slider.value = x, value, easingDuration);
		}
		fill.color = gradient.Evaluate(value/slider.maxValue);
	}
}
