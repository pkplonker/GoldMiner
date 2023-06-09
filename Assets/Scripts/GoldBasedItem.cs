//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using UnityEngine;

/// <summary>
///Item full description
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Base Item")]
public class GoldBasedItem : Item
{
	public override float GetValue() => Value * GoldPrice.goldPrice;
}