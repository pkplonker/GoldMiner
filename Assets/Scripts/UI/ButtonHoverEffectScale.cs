using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class ButtonHoverEffectScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private float scaleAmount = 1.1f;
		[SerializeField] private float scaleDuration = 0.2f;

		public void OnPointerEnter(PointerEventData eventData) => Hover();

		private void Hover() => transform.DOScale(Vector3.one * scaleAmount, 0.2f);

		public void OnPointerExit(PointerEventData eventData) => UnHover();

		private void UnHover() => transform.DOScale(Vector3.one, 0.2f);
	}
}