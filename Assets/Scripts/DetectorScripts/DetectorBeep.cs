//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectorBeep full description
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class DetectorBeep : MonoBehaviour
	{
		private float beepTimer = 0;
		[SerializeField]private float currentFrequency = 1f;

		[SerializeField]private float lowestFrequency = 1f;
		[SerializeField]private float highestFrequency = 0.3f;

		AudioSource audioSource;
		[SerializeField] private AudioClip beepClip;
		[SerializeField] private float defaultPitch=1;
		[SerializeField] private float maxPitch=1.3f;
		[Range(0,1f)][SerializeField] private float currentSignalTest = 0f;
		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			 //test
			 //var signal = currentSignalTest;
			 
			 //production
			 var signal = DetectorHead.currentSignal;
			 
			 
			if (!DetectorState.isDetecting) return;
			beepTimer += Time.deltaTime;
			audioSource.pitch = Mathf.Lerp(defaultPitch, maxPitch,signal /*DetectorHead.currentSignal*/);
			currentFrequency = Mathf.Lerp(lowestFrequency, highestFrequency,signal /*DetectorHead.currentSignal*/);

			if (beepTimer >= currentFrequency)
			{
				beepTimer = 0;
				if(signal> 0.02f)
					audioSource.PlayOneShot(beepClip);
			}

		}
	}
}