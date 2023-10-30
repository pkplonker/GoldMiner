//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEditor;
using UnityEngine;

/// <summary>
///Item full description
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Base Item")]
public class Item : ScriptableObject, ISerializationCallbackReceiver
{
	public string ItemName = "New Item";
	public string Description = "Description";
	public string GUID;

	[SerializeField] protected float Value = 10f;
	public Sprite Sprite;
	public virtual float GetValue() => Value;
	public virtual bool Use() => false;

	public void OnBeforeSerialize()
	{
#if UNITY_EDITOR
		GenerateUniqueID();
#endif
	}

	public void OnAfterDeserialize() { }

	private void GenerateUniqueID()
	{
		if (string.IsNullOrWhiteSpace(GUID))
		{
			GUID = Guid.NewGuid().ToString();
		}
	}
}