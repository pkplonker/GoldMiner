using UnityEngine;

namespace DetectorScripts
{
	public static class ToneGenerator
	{
		private static float currentFreq;

		public static AudioClip GenerateFrequency(float frequency, int sampleRate)
		{
			var clip = CreateToneClip(frequency, sampleRate);
			return clip;
		}

		private static AudioClip CreateToneClip(float frequency, int sampleRate)
		{
			AudioClip clip = AudioClip.Create(frequency.ToString(), sampleRate, 1, sampleRate, false);
			var size = clip.frequency * (int) Mathf.Ceil(clip.length);
			float[] data = new float[size];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
			}

			clip.SetData(data, 0);
			return clip;
		}

		public class ToneClipData
		{
			public float frequency;
			public AudioClip clip;

			public ToneClipData(float frequency, int sampleRate = 44100)
			{
				this.frequency = frequency;
				clip = CreateToneClip(frequency, sampleRate);
			}
		}
	}
}