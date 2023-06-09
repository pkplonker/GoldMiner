using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class InventorySlotUI : MonoBehaviour
	{
		private Item item;
		private float quantity;
		[SerializeField] private Image image;
		[SerializeField] private TextMeshProUGUI quantityText;

		public void SetItem(InventorySlot slot)
		{
			if (slot == null) return;
			item = slot._item;
			quantity = slot._quantity;
			UpdateUI();
		}

		private void UpdateUI()
		{
			SetImage();
			SetQuantity();
		}

		private void SetQuantity()
		{
			if (quantity == 0) quantityText.enabled = false;
			else
			{
				quantityText.text = quantity.ToString();
				quantityText.enabled = true;
			}
		}

		private void SetImage()
		{
			if (item == null)
			{
				image.enabled = false;
				return;
			}
			image.enabled = true;

			image.sprite = item.Sprite;
		}

		public void Use()
		{
			if (item != null)
			{
				item.Use();
			}
		}
	}
}