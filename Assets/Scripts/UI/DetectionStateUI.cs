using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class DetectionStateUI : MonoBehaviour
	{
		[SerializeField] private Image manualDetectionImage;
		[SerializeField] private Image detectionImage;

		private void OnEnable()
		{
			DetectorState.OnDetectorToggleChanged += OnDetectorToggleChanged;
			DetectorState.OnDetectorManualToggleChanged += OnDetectorManualToggleChanged;
		}

		private void OnDisable()
		{
			DetectorState.OnDetectorToggleChanged += OnDetectorToggleChanged;
			DetectorState.OnDetectorManualToggleChanged += OnDetectorManualToggleChanged;
		}

		private void OnDetectorManualToggleChanged(bool state) =>
			SetImageColor(manualDetectionImage, state ? Color.green : Color.red);

		private void OnDetectorToggleChanged(bool state) =>
			SetImageColor(detectionImage, state ? Color.green : Color.red);

		private void SetImageColor(Image image, Color color)
		{
			if (image == null) return;
			image.color = color;
		}
	}
}