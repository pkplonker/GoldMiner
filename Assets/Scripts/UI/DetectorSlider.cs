//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using DetectorScripts;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	///DetectorSlider full description
	/// </summary>
	public class DetectorSlider : MonoBehaviour
	{
		[SerializeField] private Image _image;

		private void OnEnable() => DetectorHead.OnDetection += SetFillAmount;
		private void OnDisable() => DetectorHead.OnDetection -= SetFillAmount;
		private void SetFillAmount(float i) => _image.fillAmount = i;
		
		private void Update()
		{
			if (PlayerInteractionStateMachine.IsDetecting) SetFillAmount(DetectorHead.CurrentSignal);
			else if (_image.fillAmount != 0) SetFillAmount(0);
			
		}

	}
}