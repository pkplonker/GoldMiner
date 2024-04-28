using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveTest : MonoBehaviour, ISaveLoad
{
	[SerializeField] private float test = 123;
	[SerializeField] private string testName = "steve";

	public void LoadState(object data)
	{
		if (data is JObject jobject)
		{
			try
			{
				var saveData = jobject.ToObject<SaveData>();
				test = saveData.test;
				testName = saveData.testName;
				Debug.Log("Loaded data: " + saveData.test + ", " + saveData.testName);
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
		var data = new SaveData();
		data.test = Random.value;
		data.testName = testName + ".";
		return data;
	}

	[Serializable]
	public struct SaveData
	{
		public float test;
		public string testName;
	}
}