using System;
using System.Collections;
using Player;
using TMPro;
using UnityEngine;

namespace UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField] private string _message = "Gold: ";
		[SerializeField] private string _siUnit = "g";
		[SerializeField] private bool _postUnit = true;
		[SerializeField] private float _speed = 5f;
		[SerializeField] private TextMeshProUGUI _textMeshProUGUI;
		private float _targetAmount;
		private float _currentAmount;
		private Coroutine _coroutine;
		private void Awake() => UpdateText();
		
		[SerializeField] private PlayerReference _playerReference;
		private void OnEnable() => PlayerReference.OnPlayerChanged += PlayerChanged;

		private void OnDisable()
		{
			PlayerReference.OnPlayerChanged += PlayerChanged;
			UnSubscribe();
		}

		protected virtual void UnSubscribe(){ }

		protected virtual void Subscribe() { }

		private void PlayerChanged()
		{
			if (_playerReference.GetPlayer() == null)
			{
				PlayerCurrency.OnGoldChanged -= Received;
				return;
			}
			Subscribe();
		}

		private void UpdateText()
		{
			if (_postUnit) _textMeshProUGUI.text = _message + _currentAmount.ToString("n2") + _siUnit;
			else _textMeshProUGUI.text = _message + _siUnit + _currentAmount.ToString("n2");
		}

		protected void Received(float weightFound, float total)
		{
			Debug.Log("received");
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