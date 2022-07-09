//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	///DetectorSlider full description
	/// </summary>
	public class DetectorSlider : MonoBehaviour
	{
		[SerializeField] private Image image;

		private void Start() => SetFillAmount(0);

		private void SetFillAmount(float i) => image.fillAmount = i;

		private void OnEnable() => DetectorHead.OnDetection += Detect;

		private void OnDisable() => DetectorHead.OnDetection -= Detect;

		private void Detect(float strength)
		{
			SetFillAmount(strength);
		} 
	}
}