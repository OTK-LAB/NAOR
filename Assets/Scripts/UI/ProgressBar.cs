using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ProgressBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;
	public float easingDuration = 0.5f;
	public void SetMaxValue(float value)
	{
		slider.maxValue = value;

		fill.color = gradient.Evaluate(1f);
	}
	public void SetValue(float value)
	{
		DOTween.To(() => slider.value, x => slider.value = x, value, easingDuration);
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}