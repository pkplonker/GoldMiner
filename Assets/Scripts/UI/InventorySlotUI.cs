using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class InventorySlotUI : MonoBehaviour
	{
		private Item _item;
		private float _quantity;
		[SerializeField] private Image _image;
		[SerializeField] private TextMeshProUGUI _quantityText;

		public void SetItem(InventorySlot slot)
		{
			if (slot == null) return;
			_item = slot._item;
			_quantity = slot._quantity;
			UpdateUI();
		}

		private void UpdateUI()
		{
			SetImage();
			SetQuantity();
		}

		private void SetQuantity()
		{
			if (_quantity == 0) _quantityText.enabled = false;
			else
			{
				_quantityText.text = _quantity.ToString();
				_quantityText.enabled = true;
			}
		}

		private void SetImage()
		{
			if (_item == null)
			{
				_image.enabled = false;
				return;
			}
			_image.enabled = true;

			_image.sprite = _item.Sprite;
		}

		public void Use()
		{
			if (_item != null)
			{
				_item.Use();
			}
		}
	}
}