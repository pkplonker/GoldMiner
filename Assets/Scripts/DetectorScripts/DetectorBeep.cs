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
		AudioSource _audioSource;
		ToneGenerator.ToneClipData _toneClipData;
		[SerializeField] private float _lowFreq;
		[SerializeField] private float _highFreq;

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			_toneClipData = new ToneGenerator.ToneClipData(0);
		}

		private void Update()
		{
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
	}
}