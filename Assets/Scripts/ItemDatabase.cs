using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
	[SerializeField] public List<Item> items;

	private Dictionary<string, Item> itemLookup;

	private void OnEnable()
	{
		itemLookup = new Dictionary<string, Item>();
		for (var i = 0; i < items.Count; i++)
		{
			var item = items[i];
			if (item == null)
			{
				Debug.LogError($"Null item found in ItemDatabase at index {i}.");
				continue;
			}

			if (string.IsNullOrEmpty(item.GUID))
			{
				Debug.LogError($"Item {item.name} at index {i} has a null or empty GUID.");
				continue;
			}

			if (!itemLookup.ContainsKey(item.GUID))
			{
				itemLookup.Add(item.GUID, item);
			}
			else
			{
				Debug.LogError(
					$"Duplicate GUID detected in Item Database: {item.GUID} for item {item.name} at index {i}");
			}
		}
	}

	public Item GetItemByGUID(string guid)
	{
		if (itemLookup.TryGetValue(guid, out var item))
		{
			return item;
		}

		Debug.LogError($"Item not found in database with GUID: {guid}");
		return null;
	}
}