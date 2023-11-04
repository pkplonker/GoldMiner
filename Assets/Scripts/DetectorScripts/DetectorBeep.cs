//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Audio;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace DetectorScripts
{
	/// <summary>
	///DetectorBeep full description
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class DetectorBeep : MonoBehaviour
	{
		private AudioSource audioSource;
		ToneGenerator.ToneClipData toneClipData;
		[SerializeField] private float lowFreq;
		[SerializeField] private float highFreq;
		private bool isDetecting;

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
			toneClipData = new ToneGenerator.ToneClipData(0);
		}

		private void OnEnable() => PlayerInteractionStateMachine.OnStateChanged += OnPlayerStateChanged;
		private void OnDisable() => PlayerInteractionStateMachine.OnStateChanged -= OnPlayerStateChanged;

		private void OnPlayerStateChanged(BaseState state) => isDetecting = state.GetType() == typeof(DetectorState);

		private void Update()
		{
			if (isDetecting)
			{
				var signal = DetectorHead.CurrentSignal;
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
			else
			{
				audioSource.Stop();
			}
		}
	}
}