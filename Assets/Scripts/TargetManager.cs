using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Player;
using Save;
using Targets;
using UnityEngine;

public class TargetManager : MonoBehaviour, IService, ISaveLoad, IResetOnLoad
{
	public HashSet<Target> spawnedTargets { get; private set; } = new();
	public HashSet<Target> targetPiecesFound { get; private set; } = new();
	[SerializeField] protected PlayerReference playerReference;
	public event Action<Transform> TargetRegistered;

	protected virtual void Awake()=> ServiceLocator.Instance.RegisterService(this);
	public virtual void RegisterTarget(Target target)=> spawnedTargets.Add(target);
	
	public virtual void DeregisterTarget(Target target)
	{
		if (!spawnedTargets.Contains(target)) return;
		spawnedTargets.Remove(target);
		targetPiecesFound.Add(target);
		TargetRegistered?.Invoke(target.transform);
		target.DisableObject();
	}

	public void Initialize() { }

	public void LoadState(object data)
	{
		if (data is JObject jobject)
		{
			try
			{
				var saveData = jobject.ToObject<SaveData>();

				foreach (var savedPosition in saveData.positions.Select(x => x.GetVector()))
				{
					Target closestTarget = FindClosestTarget(savedPosition);
					if (closestTarget != null)
					{
						DeregisterTarget(closestTarget);
						targetPiecesFound.Add(closestTarget);
					}
				}
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

	private Target FindClosestTarget(Vector3 position)
	{
		float minDistance = float.MaxValue;
		Target closestTarget = null;

		foreach (var target in spawnedTargets)
		{
			float distance = Vector3.Distance(position, target.Position);
			if (distance < minDistance)
			{
				minDistance = distance;
				closestTarget = target;
			}
		}

		float thresholdDistance = 0.01f;
		if (minDistance <= thresholdDistance)
		{
			return closestTarget;
		}

		return null;
	}

	public object SaveState()
	{
		var data = new SaveData
		{
			positions = new HashSet<SerializableVector>(
				targetPiecesFound.Select(x => new SerializableVector(x.Position)))
		};
		return data;
	}

	[Serializable]
	private struct SaveData
	{
		public HashSet<SerializableVector> positions;
	}

	public void Reset()
	{
		foreach (var target in targetPiecesFound)
		{
			spawnedTargets.Add(target);
			target.EnableObject();
		}
		targetPiecesFound.Clear();
	}
}