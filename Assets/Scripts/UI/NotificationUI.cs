using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
	public class NotificationUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _tmp;
		private Coroutine _disableCor;
		[SerializeField] private float _waitTime = 3f;
		private WaitForSeconds _waitForSeconds;
		private string _currentText;

		private void Awake()
		{
			_waitForSeconds = new WaitForSeconds(_waitTime);
			_tmp.gameObject.SetActive(false);
		}

		public void UpdateText(string text)
		{
			if (_disableCor != null) StopCoroutine(_disableCor);

			if (string.IsNullOrEmpty(text)) _tmp.gameObject.SetActive(false);
			else
			{
				_tmp.gameObject.SetActive(true);
				SetText(text);
				_disableCor = StartCoroutine(DisableText());
			}
		}


		private void SetText(string text)
		{
			if (text == _currentText) return;
			_tmp.text = text;
		}

		private IEnumerator DisableText()
		{
			yield return _waitForSeconds;
			_tmp.gameObject.SetActive(false);
		}
	}
}