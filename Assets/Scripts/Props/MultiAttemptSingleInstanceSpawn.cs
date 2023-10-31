using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using UnityEngine;

namespace Props
{
	[CreateAssetMenu(fileName = "New Multi-attempt Single instance spawn",
		menuName = "Props/Spawns/New Multi-attempt Single instance spawn")]
	public class MultiAttemptSingleInstanceSpawn : SingleInstanceSpawn
	{
		public GameObject Prefab;
		public float FlattnessTolerance;
		[SerializeField] private float IncrementAmount = 0.2f;
		[SerializeField] private int Attempts = 50;
		[SerializeField] private Vector3 StartOffset = Vector3.zero;
		[SerializeField] private Vector3 FinalOffset = Vector3.zero;
		[SerializeField] private bool alignToTerrain;

		protected virtual Trans CalculateSpawn(float size, GameObject currentInstance, string groundLayer)
		{
			var t = new Trans();
			var result = AlignToTerrain(CalculatePosition(size, currentInstance, groundLayer), currentInstance);
			t.Position = result.position;
			t.Rotation = result.rotation;
			return t;
		}

		protected virtual (Quaternion rotation, Vector3 position) AlignToTerrain(Vector3 position,
			GameObject currentInstance)
		{
			if (!alignToTerrain || currentInstance == null)
				return (Quaternion.identity, position);

			BoxCollider collider = currentInstance.GetComponent<BoxCollider>();
			if (collider == null)
			{
				Debug.LogError("The GameObject does not have a Collider component.");
				return (Quaternion.identity, position);
			}

			Vector3[] bottomCorners = GetBottomCorners(collider);

			Vector3[] hitPoints = new Vector3[4];
			int hitCount = 0;

			for (int i = 0; i < 4; i++)
			{
				Vector3 rayStart = position + bottomCorners[i] + Vector3.up * 10;
				//Debug.DrawRay(rayStart, Vector3.down * 20, Color.green, 5f);
				if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
				{
					hitPoints[hitCount] = hit.point;
					hitCount++;
				}
			}

			if (hitCount < 3) return (Quaternion.identity, position);

			Vector3 vector1 = hitPoints[1] - hitPoints[0];
			Vector3 vector2 = hitPoints[2] - hitPoints[0];
			Vector3 averageNormal = Vector3.Cross(vector1, vector2).normalized;

			Vector3 newPosition = CalculateNewPosition(hitPoints, hitCount);
			newPosition.y = hitPoints.Min(hit => hit.y);

			if (averageNormal.y < 0)
				averageNormal = -averageNormal;

			newPosition.y = hitPoints.Min(hit => hit.y);

			Quaternion newRotation =
				Quaternion.LookRotation(Vector3.Cross(averageNormal, currentInstance.transform.right), averageNormal);
			newPosition.y += collider.center.y;
			return (newRotation, newPosition);
		}

		private Vector3[] GetBottomCorners(BoxCollider collider)
		{
			Vector3[] corners = new Vector3[4];

			Vector3 size = collider.size;
			Vector3 center = collider.center;

			corners[0] = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
			corners[1] = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
			corners[2] = center + new Vector3(-size.x, -size.y, size.z) * 0.5f;
			corners[3] = center + new Vector3(size.x, -size.y, size.z) * 0.5f;

			return corners;
		}

		private Vector3 CalculateNewPosition(Vector3[] hitPoints, int hitCount)
		{
			Vector3 newPosition = Vector3.zero;
			for (int i = 0; i < hitCount; i++)
			{
				newPosition += hitPoints[i];
			}

			newPosition /= hitCount;
			return newPosition;
		}

		protected virtual Vector3 CalculatePosition(float size, GameObject currentInstance, string groundLayer)
		{
			var position = new Vector3(size / 2, 50, size / 2);
			position += StartOffset;
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
					position += FinalOffset;

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
			return position;
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
			var spawnTransform = CalculateSpawn(mapData.GetSize(), currentInstance, mapData.GroundLayer);

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