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
			MapGenerator.MapGenerationStarted += ProgressStarted;
			PropSpawner.OnPropGenerated += NewProp;
			MapGenerator.MapGenerated += MapGenerated;
		}
		private void OnDisable()
		{
			MapGeneratorTerrain.OnChunkGenerated -= NewChunk;
			MapGenerator.MapGenerationStarted -= ProgressStarted;
			PropSpawner.OnPropGenerated -= NewProp;
			MapGenerator.MapGenerated -= MapGenerated;
		}

		private void MapGenerated(float obj)
		{
			Destroy(gameObject);
			if(updatingCor!=null) StopCoroutine(updatingCor);
			fadeTime = 1f;
			HideUI(fadeTime);
			StopCor();
		}

		private void NewProp(int count)
		{
			currentPropProgress = count;
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
			while (progressLoadingImage.fillAmount!=1)
			{
				currentFillTarget = (float)currentTotal / totalRequired;
				progressLoadingImage.fillAmount = Mathf.Lerp(progressLoadingImage.fillAmount, currentFillTarget,
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