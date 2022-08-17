//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Audio;
using Player;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectorBeep full description
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class DetectorBeep : MonoBehaviour
	{
		AudioSource _audioSource;
		ToneGenerator.ToneClipData _toneClipData;
		[SerializeField] private float _lowFreq;
		[SerializeField] private float _highFreq;
		[SerializeField] private bool isDetecting = false;

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			_toneClipData = new ToneGenerator.ToneClipData(0);
		}

		private void OnEnable() => PlayerInteractionStateMachine.OnStateChanged += OnPlayerStateChanged;
		private void OnDisable() => PlayerInteractionStateMachine.OnStateChanged -= OnPlayerStateChanged;

		private void OnPlayerStateChanged(BaseState state) => isDetecting = state.GetType() == typeof(DetectorState);


		private void Update()
		{
			if(isDetecting){
			var signal = DetectorHead.CurrentSignal;
			if (signal < 0.1f)
			{
				_audioSource.Stop();
				return;
			}

			//determine frequency
			var freq = _lowFreq + (_highFreq - _lowFreq) * signal;
			//generate tone
			var clipData = new ToneGenerator.ToneClipData(freq);
			if (_toneClipData == null || _toneClipData.frequency == clipData.frequency) return;

			_toneClipData = clipData;
			//play tone
			//audioSource.Stop();
			_audioSource.clip = clipData.clip;
			_audioSource.Play();
			}
			else
			{
				_audioSource.Stop();
			}
		}
	}
}