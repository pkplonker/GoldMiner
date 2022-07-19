//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using DetectorScripts;
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

		private void OnEnable() => DetectorHead.OnDetection += SetFillAmount;


		private void OnDisable() => DetectorHead.OnDetection -= SetFillAmount;

		private void Update()
		{
			if (DetectorState.isDetecting)
			{
				SetFillAmount(DetectorHead.currentSignal);
			}
			else if (image.fillAmount != 0)
			{
				SetFillAmount(0);
			}
		}

		private void SetFillAmount(float i) => image.fillAmount = i;
	}
}