using System;
using System.Collections;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class MapLoadingUI : CanvasGroupBase
	{
		[SerializeField] private Image progressLoadingImage;
		[SerializeField] private float progressBarSpeed=4f;
		[SerializeField] private float fadeTime=0.3f;

		private int currentPropProgress;
		private int currentChunkProgress;
		private int requiredProp;
		private int requiredChunk;
		private int totalRequired;
		private int currentTotal;
		private float currentFillTarget;
		private Coroutine updatingCor;

		private void Awake()
		{
			HideUI();
			StopCor();
		}

		private void OnEnable()
		{
			MapGeneratorTerrain.OnChunkGenerated += NewChunk;
			MapGenerator.OnMapGenerationStarted += ProgressStarted;
			PropSpawner.OnPropGenerated += NewProp;
			MapGenerator.OnMapGenerated += MapGenerated;
		}

		private void MapGenerated(float obj)
		{
			if(updatingCor!=null) StopCoroutine(updatingCor);
			fadeTime = 1f;
			HideUI(fadeTime);
		}

		private void NewProp(int current, int target)
		{
			Debug.Log($"Chunks =  {current}/{target}");

			currentPropProgress = current;
			if(current==target) Debug.Log("props complete");
			UpdateTotals();
		}

		private void UpdateTotals()
		{
			currentTotal = currentChunkProgress + currentPropProgress;
			if (currentTotal == totalRequired)
			{
				Complete();
			}
		}

		
		private void Complete()
		{
			Debug.Log("Completed");
			StopCor();
			HideUI();
		}

		private void ProgressStarted(int chunks, int props)
		{
			currentTotal = 0;
			currentChunkProgress = 0;
			currentPropProgress = 0;
			requiredChunk = chunks;
			requiredProp = props;
			totalRequired = requiredChunk + requiredProp;
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
			progressLoadingImage.fillAmount = 0f;
		}

		private IEnumerator ProgressBarUpdateCor()
		{
			while (true)
			{
				currentFillTarget = (float)currentTotal / totalRequired;
				progressLoadingImage.fillAmount = Mathf.Lerp(progressLoadingImage.fillAmount, currentFillTarget,
					progressBarSpeed * Time.deltaTime);
				yield return null;
			}
		}

		private void NewChunk(int generated, int required)
		{
			Debug.Log($"Chunks =  {generated}/{required}");
			currentChunkProgress = generated;
			if(generated==required) Debug.Log("chunks complete");
			UpdateTotals();
		}
	}
}