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
			var collider = prop.GetComponent<Collider>();
			if (collider != null)
			{
				var bounds = collider.bounds;
				var hitPointXZ = new Vector3(hit.point.x, bounds.center.y, hit.point.z);
				var boundsCenterXZ = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);

				if (bounds.Contains(hitPointXZ))
				{
					result = false;
				}
			}
			else
			{
				var propPositionXZ = new Vector3(prop.position.x, hit.point.y, prop.position.z);
				float distance = Vector3.Distance(hit.point, propPositionXZ);
				if (distance < radius)
				{
					propsToRemove.Add(prop);
				}
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
		props = GetComponentsInChildren<Transform>().ToList();
		props = props.Where(t => t != transform && t.gameObject.CompareTag(propTag)).ToList();
		props = props.Select(t => t.transform).ToList();
	}
}