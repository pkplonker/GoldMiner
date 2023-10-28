using System;
using System.Collections.Generic;
using System.Linq;
using Targets;
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

		var propsToRemove = new List<Transform>();
		var result = true;

		foreach (var prop in props)
		{
			var collider = prop.GetComponent<Collider>();
			if (collider != null)
			{
				var bounds = collider.bounds;
				var hitPointXZ = new Vector3(hit.point.x, bounds.center.y, hit.point.z);

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
		props = GetComponentsInChildren<Transform>().Where(t => t != transform && t.gameObject.CompareTag(propTag))
			.Where(x => x.GetComponent<Target>() == null).Select(t => t.transform).ToList();
	}
}