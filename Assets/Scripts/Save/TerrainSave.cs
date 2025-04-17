using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Save;
using UnityEngine;

[RequireComponent(typeof(DiggableTerrain))]
public class TerrainSave : MonoBehaviour, ISaveLoad, IResetOnLoad
{
	[SerializeField] private DiggableTerrain diggableTerrain;
	private HashSet<DiggableTerrain.DigParams> digParams = new();

	[Serializable]
	private struct SaveData
	{
		public HashSet<DiggableTerrain.DigParams> digList;
	}

	private void OnValidate()
	{
		diggableTerrain = GetComponent<DiggableTerrain>();
	}

	private void OnEnable()
	{
		diggableTerrain.OnDig += OnDig;
	}
	private void OnDisable()
	{
		diggableTerrain.OnDig -= OnDig;
	}

	private void OnDig(DiggableTerrain.DigParams param) => digParams.Add(param);

	public void LoadState(object data)
	{
		if (data is JObject jobject)
		{
			try
			{
				var saveData = jobject.ToObject<SaveData>();
				digParams = saveData.digList;
				LoadFromParams();
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

	private void LoadFromParams()
	{
		if (digParams != null && diggableTerrain != null)
		{
			foreach (var digParam in digParams)
			{
				var param = digParam;
				param.PlayVFX = false;
				diggableTerrain.Dig(param, true);
			}
		}
	}

	public object SaveState() => new SaveData() {digList = digParams};

	public void Reset()
	{
		if (diggableTerrain != null) diggableTerrain.Reset();
	}
}