using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemLoader : UnityEditor.Editor
{
	private void OnEnable()
	{
		Item item = (Item) target;
		ItemDatabase database = GetItemDatabase();

		if (database != null && !database.items.Contains(item))
		{
			database.items.Add(item);
			EditorUtility.SetDirty(database);
		}
	}

	private ItemDatabase GetItemDatabase()
	{
		string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
		if (guids.Length > 0)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			return AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
		}

		return null;
	}
}