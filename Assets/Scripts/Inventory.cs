using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Save;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveLoad
{
	private List<InventorySlot> slots;
	[SerializeField] private int capacity = 10;
	[SerializeField] private ItemDatabase itemDatabase;
	public event Action OnInventoryChanged;
	public event Action OnInventorySetup;

	public int GetCapacity() => capacity;

	private void Start() => FillSlotCapacity();

	private void FillSlotCapacity()
	{
		slots ??= new List<InventorySlot>();
		if (slots.Capacity > capacity) slots.Capacity = capacity;
		else if (slots.Capacity < capacity) slots.Capacity = capacity;
		while (slots.Count < slots.Capacity)
		{
			slots.Add(new InventorySlot(null, 0));
		}

		OnInventorySetup?.Invoke();
	}

	public bool Add(Item item, int quantity = 1)
	{
		if (item == null) return false;
		if (slots.Any(t => t.item == item))
		{
			var slot = slots.First(t => t.item == item);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		//check for empty space
		if (slots.Any(t => t.item == null))
		{
			var slot = slots.FirstOrDefault(t => t.item == null);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		return false;
	}

	public bool Remove(Item item, int quantity = 1)
	{
		if (slots.Any(t => t.item == item))
		{
			var slot = slots.First(t => t.item == item);
			slot.Remove(quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		return false;
	}

	public InventorySlot GetItem(int i)
	{
		if (slots == null) return null;
		return i > slots.Count ? null : slots[i];
	}

	public bool RemoveAtSlot(int quantity, int index) => slots[index].Remove(quantity);

	public void ClearInventory()
	{
		for (int i = 0; i < slots.Count; i++)
		{
			slots[i].item = null;
			slots[i].quantity = 0;
		}

		OnInventoryChanged?.Invoke();
	}

	public void LoadState(object data)
	{
		if (data is JObject jobject)
		{
			try
			{
				var saveData = jobject.ToObject<InventorySaveData>();
				ClearInventory();

				foreach (var itemData in saveData.Items)
				{
					var item = itemDatabase.GetItemByGUID(itemData.ItemGUID);
					if (item != null)
					{
						Add(item, itemData.Quantity);
					}
					else
					{
						Debug.LogWarning($"Item not found: {itemData.ItemGUID}");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to deserialize SaveData: " + ex);
			}
		}
		else
		{
			Debug.LogError("Invalid data type passed to LoadState");
		}
	}

	public object SaveState()
	{
		var saveData = new InventorySaveData
		{
			Items = new List<InventorySaveData.ItemSaveData>()
		};

		foreach (var slot in slots)
		{
			if (slot.item != null)
			{
				var itemData = new InventorySaveData.ItemSaveData
				{
					ItemGUID = slot.item.GUID,
					Quantity = slot.quantity
				};
				saveData.Items.Add(itemData);
			}
		}

		return saveData;
	}

	[Serializable]
	public class InventorySaveData
	{
		public List<ItemSaveData> Items;

		[Serializable]
		public class ItemSaveData
		{
			public string ItemGUID;
			public int Quantity;
		}
	}
}

[Serializable]
public class InventorySlot
{
	public Item item;
	public int quantity;

	public InventorySlot(Item item, int quantity)
	{
		this.item = item;
		this.quantity = quantity;
	}

	public void Add(Item item, int quantity)
	{
		this.item = item;
		this.quantity += quantity;
	}

	public bool Remove(int quantity)
	{
		this.quantity -= quantity;
		switch (quantity)
		{
			case > 0:
				return true;
			case 0:
				item = null;
				return true;
			default:
				this.quantity += quantity;
				return false;
		}
	}
}