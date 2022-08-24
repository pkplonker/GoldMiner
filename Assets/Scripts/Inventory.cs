using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private List<InventorySlot> _slots ;
	[SerializeField] private int _capacity = 10;

	public event Action OnInventoryChanged;
	public event Action OnInventorySetup;

	public int GetCapacity() => _capacity;

	private void Start() => FillSlotCapacity();


	private void FillSlotCapacity()
	{
		_slots ??= new List<InventorySlot>();
		if (_slots.Capacity > _capacity) _slots.Capacity = _capacity;
		else if (_slots.Capacity < _capacity) _slots.Capacity = _capacity;
		while (_slots.Count < _slots.Capacity)
		{
			_slots.Add(new InventorySlot(null, 0));
		}
		OnInventorySetup?.Invoke();
	}

	public bool Add(Item item, int quantity = 1)
	{
		if (item == null) return false;
		if (_slots.Any(t => t._item == item))
		{
			var slot = _slots.First(t => t._item == item);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		//check for empty space
		if (_slots.Any(t => t._item == null))
		{
			var slot = _slots.FirstOrDefault(t => t._item == null);
			slot.Add(item, quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		return false;
	}

	public bool Remove(Item item, int quantity = 1)
	{
		if (_slots.Any(t => t._item == item))
		{
			var slot = _slots.First(t => t._item == item);
			slot.Remove(quantity);
			OnInventoryChanged?.Invoke();
			return true;
		}

		return false;
	}

	public InventorySlot GetItem(int i)
	{
		if (_slots == null) return null;
		return i > _slots.Count ? null : _slots[i];
	}

	public bool RemoveAtSlot(int quantity, int index) => _slots[index].Remove(quantity);
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