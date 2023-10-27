//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
using StuartHeathTools;
using UnityEngine;

namespace Targets
{
	/// <summary>
	///Gold full description
	/// </summary>
	public class Gold : Target
	{
		[SerializeField] private AnimationCurve goldChanceAnimationCurve;
		public float Weight { get; private set; }

		private void Awake()
		{
			ServiceLocator.Instance.GetService<GoldSpawnManager>().RegisterGold(this);
			var value = UtilityRandom.RandomFloat01();
			Weight = CreateWeight(goldChanceAnimationCurve, value);
			SignalStrength +=value;
			SignalStrength = Mathf.Clamp01(SignalStrength);
		}

		private static float CreateWeight(AnimationCurve goldChanceAnimationCurve, float value)=>goldChanceAnimationCurve.Evaluate(value);

		public override void Interact(PlayerInteractionStateMachine player)
		{
			Debug.Log($"Interacted with {Weight}g nugget");
			ServiceLocator.Instance.GetService<GoldSpawnManager>().GoldCollected(this);
			DisableObject();
		}
	}
}