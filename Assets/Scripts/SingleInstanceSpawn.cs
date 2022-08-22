using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

[Serializable]
public class SingleInstanceSpawn
{
	public GameObject Prefab;
	public float flattnessTolerance;
	[SerializeField] private float incrementAmount = 0.2f;
	[SerializeField] private int attempts = 50;

	public virtual Trans CalculateSpawn(float size, GameObject _currentInstance, string groundLayer)
	{
		var t = new Trans
		{
			Position = CalculatePosition(size, _currentInstance, groundLayer),
			Rotation = CalculateRotation()
		};
		return t;
	}

	private Quaternion CalculateRotation()
	{
		return Quaternion.identity;
	}

	private Vector3 CalculatePosition(float size, GameObject _currentInstance, string groundLayer)
	{
		var position = new Vector3(size / 2, 50, size / 2);
		_currentInstance.transform.position = Vector3.zero;

		for (var x = 0; x < attempts; x++)
		{
			position.x += x * incrementAmount;
			position.z += x * incrementAmount;

			var hits = Physics.RaycastAll(position, Vector3.down, 60);
			if (hits.Length == 0) continue;

			for (var i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider.transform == _currentInstance.transform) continue;
				position.y = hits[i].point.y;


				var bounds = GetBounds(_currentInstance);
				if (bounds.size == Vector3.zero)
				{
					Debug.Log("failed to get bounds");
					return Vector3.positiveInfinity;
				}

				if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
					    hits[i].point - new Vector3(0, bounds.extents.y, 0),
					    bounds, flattnessTolerance,
					    groundLayer, Quaternion.identity))) continue;

				return position;
			}
		}

		Debug.Log($"failed to spawn {Prefab.name} + " + new Vector3(size / 2, 50, size / 2));
		return Vector3.positiveInfinity;
	}

	private static Bounds GetBounds(GameObject _currentInstance)
	{
		var bounds = _currentInstance.GetComponentInChildren<Renderer>().bounds;
		return bounds;
	}

	[Serializable]
	public class Trans
	{
		public Vector3 Position;
		public Quaternion Rotation;
	}
}