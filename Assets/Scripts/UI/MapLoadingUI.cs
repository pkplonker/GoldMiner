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
		[SerializeField] private Image _progressLoadingImage;
		[SerializeField] private float _progressBarSpeed=4f;
		[SerializeField] private float _fadeTime=0.3f;

		private int _currentPropProgress;
		private int _currentChunkProgress;
		private int _requiredProp;
		private int _requiredChunk;
		private int _totalRequired;
		private int _currentTotal;
		private float _currentFillTarget;
		private Coroutine _updatingCor;

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
			if(_updatingCor!=null) StopCoroutine(_updatingCor);
			_fadeTime = 1f;
			HideUI(_fadeTime);
		}

		private void NewProp(int current, int target)
		{
			Debug.Log($"Props =  {current}/{target}");

			_currentPropProgress = current;
			if(current==target) Debug.Log("props complete");
			UpdateTotals();
		}

		private void UpdateTotals()
		{
			_currentTotal = _currentChunkProgress + _currentPropProgress;
			if (_currentTotal == _totalRequired)
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
			_currentTotal = 0;
			_currentChunkProgress = 0;
			_currentPropProgress = 0;
			_requiredChunk = chunks;
			_requiredProp = props;
			_totalRequired = _requiredChunk + _requiredProp;
			StopCor();

			_updatingCor = StartCoroutine(ProgressBarUpdateCor());
			ResetFill();
			ShowUI();
			UpdateTotals();
		}

		private void StopCor()
		{
			if (_updatingCor != null)
			{
				StopCoroutine(_updatingCor);
				_updatingCor = null;
			}
		}

		private void ResetFill()
		{
			_progressLoadingImage.fillAmount = 0f;
		}

		private IEnumerator ProgressBarUpdateCor()
		{
			while (true)
			{
				_currentFillTarget = (float)_currentTotal / _totalRequired;
				_progressLoadingImage.fillAmount = Mathf.Lerp(_progressLoadingImage.fillAmount, _currentFillTarget,
					_progressBarSpeed * Time.deltaTime);
				yield return null;
			}
		}

		private void NewChunk(int generated, int required)
		{
			Debug.Log($"Chunks =  {generated}/{required}");
			_currentChunkProgress = generated;
			if(generated==required) Debug.Log("chunks complete");
			UpdateTotals();
		}
	}
}