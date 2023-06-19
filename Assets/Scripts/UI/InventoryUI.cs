using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class InventoryUI : MonoBehaviour, IShowHideUI
	{
		[SerializeField] private GameObject InventorySlotPrefab;
		private List<InventorySlotUI> inventorySlots = new();
		private Inventory inventory;
		[SerializeField] private PlayerReference PlayerReference;
		[SerializeField] private Transform Container;
		[SerializeField] private RectTransform PanelRectTransform;
		private CanvasGroup canvasGroup;
		private bool inventoryActive ;
		[SerializeField] private float FadeTime = 0.5f;

		[SerializeField] private GameObject FirstSelectedGameObject;
		private void Awake() => PanelRectTransform = canvasGroup.GetComponent<RectTransform>();

		private void OnValidate() =>canvasGroup= GetComponent<CanvasGroup>();

		private void Start() => HideImmediate();

		private void OnEnable()
		{
			PlayerInputManager.OnInvent += ToggleInventory;
			PlayerReference.OnPlayerChanged += ChangeInventory;
		}

		private void OnDisable()
		{
			PlayerReference.OnPlayerChanged -= ChangeInventory;
			PlayerInputManager.OnInvent -= ToggleInventory;
		}

		private void ToggleInventory()
		{
			if(inventoryActive) Hide();
			else Toggle();
		}


		private void SetupInvent()
		{
			foreach (var slot in inventorySlots)
			{
				Destroy(slot.gameObject);
			}

			inventorySlots.Clear();
			for (var i = 0; i < inventory.GetCapacity(); i++)
			{
				inventorySlots.Add(Instantiate(InventorySlotPrefab, Container).GetComponent<InventorySlotUI>());
				inventorySlots[i].SetItem(inventory.GetItem(i));
			}

			if (inventorySlots.Count > 0)
			{
				EventSystem.current.SetSelectedGameObject(inventorySlots[0].gameObject);
			}
			UpdateInventory();
		}

		private void ChangeInventory()
		{
			if (PlayerReference.GetPlayer() == null) return;
			if (inventory != null)
			{
				inventory.OnInventoryChanged -= UpdateInventory;
				inventory.OnInventorySetup -= SetupInvent;
			}
			inventory = PlayerReference.GetPlayer().GetComponent<Inventory>();
			inventory.OnInventoryChanged += UpdateInventory;
			inventory.OnInventorySetup += SetupInvent;

			SetupInvent();
		}

		private void UpdateInventory()
		{
			for (var i = 0; i < inventorySlots.Count; i++)
			{
				inventorySlots[i].SetItem(inventory.GetItem(i));
			}
		}

		public void Toggle()
		{
			Enable();
			inventoryActive = true;
			canvasGroup.alpha = 0f;
			PanelRectTransform.anchoredPosition = new Vector2(PanelRectTransform.rect.width, 0);
			PanelRectTransform.DOAnchorPosX(0, FadeTime).SetEase(Ease.OutBack);
			canvasGroup.DOFade(1, FadeTime);
		}

		public void Hide()
		{
			Disable();
			Debug.Log("disabling invent");
			inventoryActive = false;
			canvasGroup.alpha = 1f;
			PanelRectTransform.anchoredPosition = new Vector2(0, 0);
			PanelRectTransform.DOAnchorPosX(PanelRectTransform.rect.width, FadeTime).SetEase(Ease.InOutQuint);
			canvasGroup.DOFade(0, FadeTime);
		}

		private void HideImmediate()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		public void Disable()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		public void Enable()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
	}
}