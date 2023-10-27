using System;
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
	private GameObject fenceParent;

	public override bool Spawn(MapData mapData, out GameObject currentInstance)
	{
		CalculatePoints(mapData);
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

			var previousPost = Vector3.zero;
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

					if (j != quantity) barScale.z *= barLength / BarLength;
					else barScale.z *= remainder / BarLength;
					
					bar.transform.localScale = barScale;
					if (j != quantity) continue;
					bar.transform.position = ((position - previousPost) / 2) + previousPost + new Vector3(0, height, 0);
				}

				previousPost = position;
			}
		}

		SpawnCollider();
		currentInstance = fenceParent;
		StaticBatchingUtility.Combine(fenceParent);
		return true;
	}
	

	public override string GetName() => "Fence";

	private void CalculatePoints(MapData mapData)
	{
		Points ??= new();
		Points.Clear();
		var small = mapData.boundryInstep;
		var large = mapData.GetSize() - small;
		Points.Add(new Vector3(small, 0, large));
		Points.Add(new Vector3(large, 0, large));
		Points.Add(new Vector3(large, 0, small));
		Points.Add(new Vector3(small, 0, small));
	}

	private void SpawnCollider()
	{
		for (var i = 0; i < Points.Count; i++)
		{
			var startPoint = Points[i];
			var endPoint = Points[(i + 1) % Points.Count];
			var direction = (endPoint - startPoint).normalized;
			var distance = Vector3.Distance(startPoint, endPoint);
        
			var colliderPosition = (startPoint + endPoint) / 2;
			colliderPosition.y = GetTerrainHeight(colliderPosition);
        
			var colliderRotation = Quaternion.LookRotation(direction);
        
			var colliderSize = new Vector3(0.1f, 20f, distance); // Adjust the Y value as needed
        
			var colliderObject = new GameObject($"EdgeCollider_{i}");
			var boxCollider = colliderObject.AddComponent<BoxCollider>();
			boxCollider.size = colliderSize;
			colliderObject.transform.position = colliderPosition;
			colliderObject.transform.rotation = colliderRotation;
			colliderObject.transform.parent = fenceParent.transform;
		}
	}


	private float GetTerrainHeight(Vector3 position)
	{
		var ray = new Ray(position + Vector3.up * 1000f, Vector3.down);
		return Physics.Raycast(ray, out RaycastHit hitInfo) ? hitInfo.point.y : position.y;
	}
}