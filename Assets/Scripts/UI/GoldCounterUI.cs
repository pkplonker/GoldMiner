using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GoldCounterUI : MonoBehaviour
	{
		[SerializeField] private string _message = "Gold: ";
		[SerializeField] private string _siUnit = "g";
		[SerializeField] private float _speed = 5f;
		[SerializeField] private TextMeshProUGUI _textMeshProUGUI;
		private float _targetAmount;
		private float _currentAmount;

		private Coroutine _coroutine;

		private void OnEnable() => GoldSpawnManager.OnGoldReceived += GoldReceived;
		private void OnDisable() => GoldSpawnManager.OnGoldReceived -= GoldReceived;

		private void Awake() => UpdateText();
		private void UpdateText() => _textMeshProUGUI.text = _message + _currentAmount.ToString("n2") + _siUnit;

		private void GoldReceived(float weightFound, float total, Vector3 position)
		{
			_targetAmount = total;

			if (_coroutine == null)
			{
				StartCoroutine(UpdateTextCor());
			}
		}

		private IEnumerator UpdateTextCor()
		{
			while (_targetAmount.ToString("n2") != _currentAmount.ToString("n2"))
			{
				_currentAmount = Mathf.MoveTowards(_currentAmount, _targetAmount, _speed * Time.deltaTime);
				UpdateText();
				yield return null;
			}

			_coroutine = null;
		}
	}
}