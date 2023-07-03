using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

namespace Props
{
	[CreateAssetMenu(fileName = "New Multi-attempt Single instance spawn", menuName = "Props/Spawns/New Multi-attempt Single instance spawn")]
	public class MultiAttemptSingleInstanceSpawn : SingleInstanceSpawn
	{
		public GameObject Prefab;
		public float FlattnessTolerance;
		[SerializeField] private float IncrementAmount = 0.2f;
		[SerializeField] private int Attempts = 50;
		[SerializeField] private Vector3 Offset = Vector3.zero;

		protected virtual Trans CalculateSpawn(float size, GameObject currentInstance, string groundLayer)
		{
			var t = new Trans
			{
				Position = CalculatePosition(size, currentInstance, groundLayer),
				Rotation = CalculateRotation()
			};
			return t;
		}

		private Quaternion CalculateRotation() => Quaternion.identity;

		private Vector3 CalculatePosition(float size, GameObject currentInstance, string groundLayer)
		{
			var position = new Vector3(size / 2, 50, size / 2);
			position += Offset;
			currentInstance.transform.position = Vector3.zero;

			for (var x = 0; x < Attempts; x++)
			{
				position.x += x * IncrementAmount;
				position.z += x * IncrementAmount;

				var hits = Physics.RaycastAll(position, Vector3.down, 60);
				if (hits.Length == 0) continue;

				for (var i = 0; i < hits.Length; i++)
				{
					if (hits[i].collider.transform == currentInstance.transform) continue;
					position.y = hits[i].point.y;

					var bounds = GetBounds(currentInstance);
					if (bounds.size == Vector3.zero)
					{
						Debug.Log("failed to get bounds");
						return Vector3.positiveInfinity;
					}

					if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
						    hits[i].point - new Vector3(0, bounds.extents.y, 0),
						    bounds, FlattnessTolerance,
						    groundLayer, Quaternion.identity))) continue;

					return position;
				}
			}

			Debug.Log($"failed to spawn {Prefab.name} + " + new Vector3(size / 2, 50, size / 2));
			return Vector3.positiveInfinity;
		}

		private static Bounds GetBounds(GameObject currentInstance)
		{
			var bounds = currentInstance.GetComponentInChildren<Renderer>().bounds;
			return bounds;
		}

		protected virtual void Setup(GameObject obj) { }

		[Serializable]
		public class Trans
		{
			public Vector3 Position;
			public Quaternion Rotation;
		}

		public override bool Spawn(MapData mapData, out GameObject currentInstance)
		{
			currentInstance = Instantiate(Prefab);
			currentInstance.SetActive(false);
			var spawnTransform = CalculateSpawn(mapData.GetSize(), currentInstance, mapData.groundLayer);

			if (spawnTransform.Position.x < mapData.GetSize())
			{
				currentInstance.transform.position = spawnTransform.Position;
				currentInstance.transform.rotation = spawnTransform.Rotation;
				currentInstance.SetActive(true);
				Setup(currentInstance);
				return true;
			}

			return false;
		}

		public override string GetName() => Prefab.name;
	}
}