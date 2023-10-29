using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using UnityEngine;

namespace Save
{
	public class SavingSystem : MonoBehaviour, IService
	{
		private List<SaveableGameObject> saveableObjects;
		[SerializeField] private string fileName = "Game.data";
		private ISaveLoadIO output;
		public event Action OnSave;
		public event Action OnLoad;

		private void Awake()
		{
			ServiceLocator.Instance.RegisterService(this);

			output = new SaveLoadIOMediator().CreateSaveLoadExternal(fileName);
		}

		private void OnEnable()
		{
			saveableObjects = new List<SaveableGameObject>();
		}

		private void Start()
		{
			//LoadGame();
		}

		/// <summary>
		/// Method <c>ClearSave</c> Public function to clear existing save file
		/// </summary>
		public void ClearSave()
		{
			output ??= new SaveFileHandler(Application.persistentDataPath, fileName);
			output.Clear();
		}

		/// <summary>
		/// Method <c>LoadGame</c> Public function to load existing save file
		/// </summary>
		public void LoadGame()
		{
			LoadData(output.Load());
		}

		/// <summary>
		/// Method <c>SaveGame</c> Public function to save new changes
		/// </summary>
		public void SaveGame(bool isNew = false)
		{
			Dictionary<string, object> existingSaveData = output.Load() ?? new Dictionary<string, object>();
			Dictionary<string, object> currentSaveData = new Dictionary<string, object>();

			SaveData(currentSaveData);

			if (!isNew)
			{
				foreach (var item in currentSaveData)
				{
					existingSaveData[item.Key] = item.Value;
				}
			}
			else
			{
				existingSaveData = currentSaveData;
			}

			output.Save(existingSaveData);
		}

		private void OnApplicationQuit()
		{
			//SaveGame();
		}

		/// <summary>
		/// Method <c>LoadData</c> Private function to distribute data to saveableobjects
		/// </summary>
		private void LoadData(Dictionary<string, object> data)
		{
			foreach (var saveableObject in saveableObjects)
			{
				saveableObject.PreLoadStep();
			}

			foreach (var saveableObject in saveableObjects)
			{
				if (saveableObject == null)
				{
					saveableObjects.Remove(saveableObject);
					continue;
				}

				if (data.TryGetValue(saveableObject.id, out object saveData))
				{
					saveableObject.LoadState(saveData);
				}
			}

			OnLoad?.Invoke();
		}

		/// <summary>
		/// Method <c>SaveData</c> Private function to collate save data from saveableObjects
		/// </summary>
		private void SaveData(Dictionary<string, object> data)
		{
			foreach (var saveableObject in saveableObjects)
			{
				if (saveableObject == null)
				{
					saveableObjects.Remove(saveableObject);
					continue;
				}

				data[saveableObject.id] = saveableObject.SaveState();
			}

			OnSave?.Invoke();
		}

		/// <summary>
		/// Method <c>Subscribe</c> Public observer pattern subscription
		/// </summary>
		public void Subscribe(SaveableGameObject saveLoadInterface)
		{
			if (!saveableObjects.Contains(saveLoadInterface))
			{
				Debug.Log("Registed");
				saveableObjects.Add(saveLoadInterface);
			}
		}

		/// <summary>
		/// Method <c>UnSubscribe</c> Public observer pattern unsubscription
		/// </summary>
		public void UnSubscribe(SaveableGameObject saveLoadInterface)
		{
			if (saveableObjects.Contains(saveLoadInterface))
			{
				saveableObjects.Remove(saveLoadInterface);
			}
		}

		public void Initialize() { }
	}
}