//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
using StuartHeathTools;
using UnityEngine;

/// <summary>
///Gold full description
/// </summary>
public class Gold : Target
{
	[SerializeField] private AnimationCurve goldChanceAnimationCurve;
	public float Weight { get; private set; }

	private void Awake()
	{
		GoldSpawnManager.Instance.RegisterGold(this);
		Weight = CreateWeight(goldChanceAnimationCurve);
	}

	private static float CreateWeight(AnimationCurve goldChanceAnimationCurve)=>goldChanceAnimationCurve.Evaluate(UtilityRandom.RandomFloat01());
	

	protected override void SetColor()=>Gizmos.color = Color.blue;
	

	public override bool Interact(PlayerInteractionStateMachine player)
	{
		Debug.Log($"Interacted with {Weight}g nugget");
		GoldSpawnManager.Instance.GoldCollected(this);
		DisableObject();
		return true;
	}

	private void DisableObject()
	{
		GetComponent<Collider>().enabled = false;
		GetComponent<MeshRenderer>().enabled = false;
	}
}