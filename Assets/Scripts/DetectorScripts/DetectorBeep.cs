//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Audio;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectorBeep full description
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class DetectorBeep : MonoBehaviour
	{
		AudioSource audioSource;
		ToneGenerator.ToneClipData toneClipData;
		[SerializeField] private float lowFreq;
		[SerializeField] private float highFreq;

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
			toneClipData = new ToneGenerator.ToneClipData(0);
		}

		private void Update()
		{
			var signal = DetectorHead.currentSignal;
			if (signal < 0.1f)
			{
				audioSource.Stop();
				return;
			}

			//determine frequency
			var freq = lowFreq + (highFreq - lowFreq) * signal;
			//generate tone
			var clipData = new ToneGenerator.ToneClipData(freq);
			if (toneClipData == null || toneClipData.frequency == clipData.frequency) return;

			toneClipData = clipData;
			//play tone
			//audioSource.Stop();
			audioSource.clip = clipData.clip;
			audioSource.Play();
		}
	}
}