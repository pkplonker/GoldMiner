using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private List<InventorySlot> slots ;
	[SerializeField] private int capacity = 10;

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
		if (slots.Any(t => t._item == item))
		{
			var slot = slots.First(t => t._item == item);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		//check for empty space
		if (slots.Any(t => t._item == null))
		{
			var slot = slots.FirstOrDefault(t => t._item == null);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		return false;
	}

	public bool Remove(Item item, int quantity = 1)
	{
		if (slots.Any(t => t._item == item))
		{
			var slot = slots.First(t => t._item == item);
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
}

[Serializable]
public class InventorySlot
{
	public Item _item;
	public int _quantity;

	public InventorySlot(Item item, int quantity)
	{
		_item = item;
		_quantity = quantity;
	}

	public void Add(Item item, int quantity)
	{
		_item = item;
		_quantity += quantity;
	}

	public bool Remove(int quantity)
	{
		_quantity -= quantity;
		switch (quantity)
		{
			case > 0:
				return true;
			case 0:
				_item = null;
				return true;
			default:
				_quantity += quantity;
				return false;
		}
	}
}