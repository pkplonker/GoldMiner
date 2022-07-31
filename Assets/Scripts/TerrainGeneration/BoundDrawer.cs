using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class BoundDrawer : MonoBehaviour
{
	public void OnDrawGizmosSelected()
	{
		if (TryGetComponent(out Renderer renderer))
		{
			Draw(renderer.bounds);
		}
		else if (TryGetComponent(out LODGroup lod))
		{
			Draw(lod.GetLODs()[0].renderers[0].bounds);
		}
	}

	public static Bounds GetBounds(GameObject gameObject)
	{
		if (gameObject.TryGetComponent(out Renderer renderer)) return renderer.bounds;
		return gameObject.TryGetComponent(out LODGroup lod) ? lod.GetLODs()[0].renderers[0].bounds : new Bounds();
	}

	private static void Draw(Bounds bounds)
	{
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
		var x = CalculateCornerBounds(bounds);
		for (int i = 0; i < x.Count; i++)
		{
			Gizmos.DrawWireSphere(x[i], 0.2f);
		}
	}

	private static List<Vector3> CalculateCornerBounds(Bounds bounds)
	{
		var points = new List<Vector3>(5);
		points.Add(bounds.center - new Vector3(0, bounds.extents.y, 0));
		points.Add(points[0] + new Vector3(bounds.extents.x, 0, bounds.extents.z));
		points.Add(points[0] + new Vector3(-bounds.extents.x, 0, -bounds.extents.z));
		points.Add(points[0] + new Vector3(-bounds.extents.x, 0, +bounds.extents.z));
		points.Add(points[0] + new Vector3(+bounds.extents.x, 0, -bounds.extents.z));

		return points;
	}

	public static bool DetermineIfGeometryIsFlat(GeometryFlatData g)
	{
		g.boundsCentreInWorldSpace += new Vector3(0, g.bounds.extents.y, 0);
		var rayStartPoints = CalculateCornerBounds(g.bounds);
		var groundMask = LayerMask.NameToLayer(g.groundLayer);
		for (var i = 0; i < rayStartPoints.Count; i++)
		{
			rayStartPoints[i] = g.rotation * rayStartPoints[i];


			Physics.Raycast((g.boundsCentreInWorldSpace + rayStartPoints[i] + new Vector3(0, g.tolerance / 2, 0)),
				Vector3.down,
				out var hit, g.tolerance);

			if (!hit.collider) return false;


			if (hit.collider.gameObject.layer != groundMask) return false;
		}


		return true;
	}

	public struct GeometryFlatData
	{
		public Vector3 boundsCentreInWorldSpace;
		public Bounds bounds;
		public float tolerance;
		public string groundLayer;
		public Quaternion rotation;

		public GeometryFlatData(Vector3 boundsCentreInWorldSpace, Bounds bounds, float tolerance, string groundLayer,
			Quaternion rotation)
		{
			this.boundsCentreInWorldSpace = boundsCentreInWorldSpace;
			this.bounds = bounds;
			this.tolerance = tolerance;
			this.groundLayer = groundLayer;
			this.rotation = rotation;
		}
	}
}