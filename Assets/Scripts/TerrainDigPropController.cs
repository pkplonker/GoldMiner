using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainDigPropController : MonoBehaviour
{
	private List<Transform> props;
	private readonly string propTag = "StaticProp";
	private const float radius = .4f;

	private void Start() => UpdateProps();

	public Action CanDig(RaycastHit hit)
	{
		UpdateProps();
		Debug.Log($"Props count {props.Count}");

		var propsToRemove = new List<Transform>();
		var result = true;

		foreach (var prop in props)
		{
			float distance;
			var collider = prop.GetComponent<Collider>();
			if (collider != null)
			{
				var closestPoint = collider.ClosestPoint(hit.point);
				distance = Vector3.Distance(hit.point, closestPoint);
			}
			else
			{
				distance = Vector3.Distance(hit.point, prop.position);
			}

			if (distance < radius)
			{
				if (collider == null) propsToRemove.Add(prop);
				else result = false;
			}
		}

		return result ? () => DestroyProps(propsToRemove) : null;
	}

	private void DestroyProps(List<Transform> propsToRemove)
	{
		foreach (var prop in propsToRemove)
		{
			props.Remove(prop);
			Destroy(prop.gameObject);
		}
	}

	private void UpdateProps()
	{
		if (props != null && props.Any()) return;
		props = GetComponentsInChildren<Transform>().ToList();
		props = props.Where(t => t != transform && t.gameObject.CompareTag(propTag)).ToList();
		props = props.Select(t => t.transform).ToList();
	}
}