using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TerrainGeneration;
using UnityEngine;

namespace Save
{
	public class SaveableGameObject : MonoBehaviour, ISerializationCallbackReceiver
	{
		public string id;
		[SerializeField] private bool PositionBasedID;

		private void Start()
		{
			if (PositionBasedID) DeterministicSeededGuid(ServiceLocator.Instance.GetService<MapGenerator>().GetSeed());
			if (ServiceLocator.Instance.GetService<SavingSystem>() == null) return;
			ServiceLocator.Instance.GetService<SavingSystem>().Subscribe(this);
		}

		private void OnDisable()
		{
			if (ServiceLocator.Instance.GetService<SavingSystem>() == null) return;
			ServiceLocator.Instance.GetService<SavingSystem>().UnSubscribe(this);
		}

		public void OnBeforeSerialize()
		{
			GenerateUniqueID();
		}

		public void DeterministicSeededGuid(int seed)
		{
			string inputString = transform.position + seed.ToString();
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(inputString);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				id = new Guid(hashBytes).ToString();
			}
		}

		/// <summary>
		/// Method <c>GenerateUniqueID</c> Private function to generate unique ID if one does not exist
		/// </summary>
		private void GenerateUniqueID()
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				id = Guid.NewGuid().ToString();
			}
		}

		//required for ISerializationCallbackReceiver interface
		public void OnAfterDeserialize() { }

		/// <summary>
		/// Method <c>SaveState</c> Public function that gets all components implementing ISaveLoad on this object add adds saveData to return dictionary
		/// </summary>
		public Dictionary<string, object> SaveState()
		{
			var saveData = new Dictionary<string, object>();
			foreach (var component in GetComponents<ISaveLoad>())
			{
				saveData[component.GetType().ToString()] = component.SaveState();
			}

			return saveData;
		}

		/// <summary>
		/// Method <c>LoadState</c> Public function that gets all components implementing ISaveLoad on this object add disseminates saveData to them
		/// </summary>
		public void LoadState(object data)
		{
			if (data is JObject jObjectData)
			{
				var components = GetComponents<ISaveLoad>();
				Dictionary<string, object> saveData = jObjectData.ToObject<Dictionary<string, object>>();
				if (saveData == null)
				{
					Debug.LogWarning($"Failed to load data on {gameObject.name}");
					return;
				}

				foreach (var component in components)
				{
					string typeName = component.GetType().ToString();
					if (saveData.TryGetValue(typeName, out object componentSaveData))
					{
						component.LoadState(componentSaveData);
					}
				}
			}
			else
			{
				Debug.LogWarning($"Invalid data type passed to LoadState on {gameObject.name}");
			}
		}

		public void PreLoadStep()
		{
			var components = GetComponents<ISaveLoad>();
			foreach (var saveLoad in components.Where(x => x is ResetOnLoad))
			{
				var component = (ResetOnLoad) saveLoad;
				component.Reset();
			}
		}
	}
}