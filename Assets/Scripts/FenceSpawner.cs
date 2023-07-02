using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;
[CreateAssetMenu(fileName = "Fence instance spawn", menuName = "Props/Spawns/Fence instance spawn")]

public class FenceSpawner : SingleInstanceSpawn
{
	[SerializeField] private GameObject PostPrefab;
	[SerializeField] private GameObject BarPrefab;
	[SerializeField] private float BarLength;
	private List<Vector3> Points;
	[SerializeField] private Vector3 barRaycastOffset;
	[SerializeField] private float postInsertionDepth = 0.2f;
	[SerializeField] private List<float> Heights;
	[SerializeField] private float fencePositionOffsetFromEdge = 50f;
	private GameObject fenceParent;

	public override bool Spawn(float size, string groundLayer, out GameObject currentInstance)
	{
		CalculatePoints(size);
		if (Points.Count == 0 || Heights.Count == 0) throw new ArgumentNullException();
		fenceParent = new GameObject("FenceParent");
		var postInsertion = new Vector3(0, postInsertionDepth, 0);
		for (var i = 0; i < (Points.Count > 2 ? Points.Count : 1); i++)
		{
			var startPoint = Points[i];
			var endPoint = Points[(i + 1) % Points.Count];
			var direction = (endPoint - startPoint).normalized;
			var distance = Vector3.Distance(startPoint, endPoint);
			var quantity = Mathf.CeilToInt(distance / BarLength);
			var remainder = distance % BarLength;

			Vector3 previousPost = Vector3.zero;
			for (var j = 0; j <= quantity; j++)
			{
				var positionVector = direction * (j * BarLength);
				positionVector = Vector3.ClampMagnitude(positionVector, distance);
				var position = startPoint + positionVector;
				position.y = GetTerrainHeight(position);
				if (j != quantity)
				{
					Instantiate(PostPrefab, position - postInsertion, Quaternion.identity, fenceParent.transform);
				}

				if (previousPost == Vector3.zero)
				{
					previousPost = position;
					continue;
				}

				foreach (var height in Heights)
				{
					var barPosition = ((position - previousPost) / 2) + previousPost;
					barPosition.y += height;
					var barDirection = (position - previousPost).normalized;
					var barLength = Mathf.Sqrt(Mathf.Pow(position.x - previousPost.x, 2) +
					                           Mathf.Pow(position.y - previousPost.y, 2) +
					                           Mathf.Pow(position.z - previousPost.z, 2));

					Vector3 raycastOrigin = previousPost + new Vector3(0, height, 0) - barRaycastOffset;
					RaycastHit[] hits = Physics.RaycastAll(raycastOrigin, barDirection, barLength);
					//Debug.DrawLine(raycastOrigin,raycastOrigin+(barDirection*barLength),Color.red,10);

					if (hits.Length > 0)
					{
						continue;
					}

					var barRotation = Quaternion.LookRotation(barDirection);
					var bar = Instantiate(BarPrefab, barPosition, barRotation, fenceParent.transform);
					var barScale = bar.transform.localScale;

					if (j != quantity)
					{
						barScale.z *= barLength / BarLength;
					}
					else
					{
						barScale.z *= remainder / BarLength;
					}

					bar.transform.localScale = barScale;

					if (j != quantity) continue;

					bar.transform.position = ((position - previousPost) / 2) + previousPost + new Vector3(0, height, 0);
				}

				previousPost = position;
			}
		}

		SpawnCollider();
		currentInstance = fenceParent;
		return true;
	}

	public override string GetName() => "Fence";

	private void CalculatePoints(float size)
	{
		Points ??= new();
		Points.Clear();
		var pos = size - fencePositionOffsetFromEdge;
		Points.Add(new Vector3(fencePositionOffsetFromEdge, 0, pos));
		Points.Add(new Vector3(pos, 0, pos));
		Points.Add(new Vector3(pos, 0, fencePositionOffsetFromEdge));
		Points.Add(new Vector3(fencePositionOffsetFromEdge, 0, fencePositionOffsetFromEdge));
	}

	private void SpawnCollider() { }

	private float GetTerrainHeight(Vector3 position)
	{
		Ray ray = new Ray(position + Vector3.up * 1000f, Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hitInfo))
		{
			return hitInfo.point.y;
		}
		else
		{
			return position.y;
		}
	}
}