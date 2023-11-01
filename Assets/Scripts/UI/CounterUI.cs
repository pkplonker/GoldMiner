using System;
using System.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField] private string message = "Gold: ";
		[SerializeField] private string siUnit = "g";
		[SerializeField] private bool postUnit = true;
		[SerializeField] private float speed = 2.5f;
		[SerializeField] private TextMeshProUGUI textMeshProUGUI;
		private float targetAmount;
		private float currentAmount;
		private Coroutine coroutine;
		private void Awake() => UpdateText();

		[SerializeField] private PlayerReference playerReference;
		private void OnEnable() => PlayerReference.OnPlayerChanged += PlayerChanged;

		private void OnDisable()
		{
			PlayerReference.OnPlayerChanged -= PlayerChanged;
			UnSubscribe();
		}

		protected virtual void UnSubscribe() { }

		protected virtual void Subscribe() { }

		private void PlayerChanged()
		{
			if (playerReference.GetPlayer() == null)
			{
				PlayerCurrency.OnGoldChanged -= Received;
				return;
			}

			Subscribe();
		}

		private void UpdateText()
		{
			if (postUnit) textMeshProUGUI.text = message + currentAmount.ToString("n2") + siUnit;
			else textMeshProUGUI.text = message + siUnit + currentAmount.ToString("n2");
		}

		protected void Received(float weightFound, float total)
		{
			targetAmount = total;
			if (coroutine == null)
			{
				StartCoroutine(UpdateTextCor());
			}
		}

		private IEnumerator UpdateTextCor()
		{
			float tolerance = 0.01f;
			var delta = targetAmount - currentAmount;
			while (Mathf.Abs(targetAmount - currentAmount) > tolerance)
			{
				currentAmount = Mathf.MoveTowards(currentAmount, targetAmount,
					targetAmount > currentAmount ? delta / speed * Time.deltaTime : -delta / speed * Time.deltaTime);
				UpdateText();
				yield return null;
			}
			currentAmount = targetAmount;
			UpdateText();
			coroutine = null;
		}
	}
}