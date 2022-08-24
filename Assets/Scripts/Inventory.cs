using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
	public static Action OnInventoryChanged;
	public bool Add(Item item)
	{
		return true;
	}
}

public class InventoryItem
{
	public Item _item;
	public int amount;
}