using System.Collections;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class MapLoadingUI : CanvasGroupBase
	{
		[SerializeField]
		private Slider slider;

		[SerializeField]
		private float progressBarSpeed = 4f;

		[SerializeField]
		private float fadeTime = 0.3f;

		private int currentChunkProgress;
		private int totalRequired;
		private float currentFillTarget;
		private Coroutine updatingCor;

		private void Awake()
		{
			HideUI();
			StopCor();
			ServiceLocator.Instance.GetService<ChunkManager>().OnChunkGeneratedAction += NewChunk;
			ServiceLocator.Instance.GetService<ChunkManager>().MapGenerationStarted += ProgressStarted;
			ServiceLocator.Instance.GetService<ChunkManager>().MapGenerated += MapGenerated;
		}

		private void OnDisable()
		{
			ServiceLocator.Instance.GetService<ChunkManager>().OnChunkGeneratedAction -= NewChunk;
			ServiceLocator.Instance.GetService<ChunkManager>().MapGenerationStarted -= ProgressStarted;
			ServiceLocator.Instance.GetService<ChunkManager>().MapGenerated -= MapGenerated;
		}

		private void MapGenerated()
		{
			Destroy(gameObject);
			if (updatingCor != null) StopCoroutine(updatingCor);
			fadeTime = 1f;
			HideUI(fadeTime);
			StopCor();
		}

		private void UpdateTotals()
		{
			if (currentChunkProgress == totalRequired)
			{
				Complete();
			}
		}

		private void Complete()
		{
			StopCor();
			HideUI();
		}

		private void ProgressStarted(int chunks)
		{
			currentChunkProgress = 0;
			totalRequired = chunks;
			StopCor();

			updatingCor = StartCoroutine(ProgressBarUpdateCor());
			ResetFill();
			ShowUI();
			UpdateTotals();
		}

		private void StopCor()
		{
			if (updatingCor != null)
			{
				StopCoroutine(updatingCor);
				updatingCor = null;
			}
		}

		private void ResetFill()
		{
			slider.value = 0f;
		}

		private IEnumerator ProgressBarUpdateCor()
		{
			while (slider.value != 1f)
			{
				currentFillTarget = (float) currentChunkProgress / totalRequired;
				slider.value = Mathf.Lerp(slider.value, currentFillTarget,
					progressBarSpeed * Time.deltaTime);
				yield return null;
			}
		}

		private void NewChunk(int generated, int required)
		{
			currentChunkProgress = generated;
			UpdateTotals();
		}
	}
}