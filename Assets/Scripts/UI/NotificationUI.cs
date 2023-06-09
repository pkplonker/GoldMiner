using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
	public class NotificationUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI tmp;
		private Coroutine disableCor;
		[SerializeField] private float waitTime = 3f;
		private WaitForSeconds waitForSeconds;
		private string currentText;

		private void Awake()
		{
			waitForSeconds = new WaitForSeconds(waitTime);
			tmp.gameObject.SetActive(false);
		}

		public void UpdateText(string text)
		{
			if (disableCor != null) StopCoroutine(disableCor);

			if (string.IsNullOrEmpty(text)) tmp.gameObject.SetActive(false);
			else
			{
				tmp.gameObject.SetActive(true);
				SetText(text);
				disableCor = StartCoroutine(DisableText());
			}
		}


		private void SetText(string text)
		{
			if (text == currentText) return;
			tmp.text = text;
		}

		private IEnumerator DisableText()
		{
			yield return waitForSeconds;
			tmp.gameObject.SetActive(false);
		}
	}
}