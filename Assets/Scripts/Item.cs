//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using UnityEngine;

/// <summary>
///Item full description
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Base Item")]
public class Item : ScriptableObject
{
	public string ItemName = "New Item";
	public string Description = "Description";

	[SerializeField] protected float Value = 10f;
	public Sprite Sprite;
	public virtual float GetValue() => Value;
	public virtual bool Use() => false;
}