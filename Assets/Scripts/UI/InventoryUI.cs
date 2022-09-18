using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class InventoryUI : MonoBehaviour, IFadeUI
	{
		[SerializeField] private GameObject _inventorySlotPrefab;
		private List<InventorySlotUI> _inventorySlots = new();
		private Inventory _inventory;
		[SerializeField] private PlayerReference _playerReference;
		[SerializeField] private Transform _container;
		private RectTransform _panelRectTransform;
		[SerializeField] private CanvasGroup _canvasGroup;

		private void Awake() => _panelRectTransform = _canvasGroup.GetComponent<RectTransform>();
		private void OnEnable() => PlayerReference.OnPlayerChanged += ChangeInventory;
		private void OnDisable() => PlayerReference.OnPlayerChanged -= ChangeInventory;


		private void SetupInvent()
		{
			foreach (var slot in _inventorySlots)
			{
				Destroy(slot.gameObject);
			}

			_inventorySlots.Clear();
			for (var i = 0; i < _inventory.GetCapacity(); i++)
			{
				_inventorySlots.Add(Instantiate(_inventorySlotPrefab, _container).GetComponent<InventorySlotUI>());
				_inventorySlots[i].SetItem(_inventory.GetItem(i));
			}
			UpdateInventory();
		}

		private void ChangeInventory()
		{
			if (_playerReference.GetPlayer() == null) return;
			if (_inventory != null)
			{
				_inventory.OnInventoryChanged -= UpdateInventory;
				_inventory.OnInventorySetup -= SetupInvent;
			}
			_inventory = _playerReference.GetPlayer().GetComponent<Inventory>();
			_inventory.OnInventoryChanged += UpdateInventory;
			_inventory.OnInventorySetup += SetupInvent;

			SetupInvent();
		}

		private void UpdateInventory()
		{
			for (var i = 0; i < _inventorySlots.Count; i++)
			{
				_inventorySlots[i].SetItem(_inventory.GetItem(i));
			}
		}

		public void FadeIn(float fadeTime)
		{
			EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);

			_canvasGroup.alpha = 0f;
			_panelRectTransform.anchoredPosition = new Vector2(_panelRectTransform.rect.width, 0);
			_panelRectTransform.DOAnchorPosX(0, fadeTime).SetEase(Ease.OutBack);
			_canvasGroup.DOFade(1, fadeTime);
		}

		public void FadeOut(float fadeTime)
		{
			_canvasGroup.alpha = 1f;
			_panelRectTransform.anchoredPosition = new Vector2(0, 0);
			_panelRectTransform.DOAnchorPosX(_panelRectTransform.rect.width, fadeTime).SetEase(Ease.InOutQuint);
			_canvasGroup.DOFade(0, fadeTime);
		}
	}
}