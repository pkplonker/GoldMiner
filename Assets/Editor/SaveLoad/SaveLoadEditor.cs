using Player;
using Save;
using UnityEditor;
using UnityEngine;

public class SaveLoadEditor : MonoBehaviour
{
	[MenuItem("SaveLoad/Save")]
	static void SaveGame()
	{
		var savingSystem = ServiceLocator.Instance.GetService<SavingSystem>();
		if (savingSystem == null) return;

		savingSystem.SaveGame();
	}

	[MenuItem("SaveLoad/Load")]
	static void LoadGame()
	{
		var savingSystem = ServiceLocator.Instance.GetService<SavingSystem>();
		if (savingSystem == null) return;

		savingSystem.LoadGame();
	}

	[MenuItem("SaveLoad/ClearSaveData")]
	static void NewGame()
	{
		var savingSystem = ServiceLocator.Instance.GetService<SavingSystem>();
		if (savingSystem == null) return;

		savingSystem.ClearSave();
	}
}