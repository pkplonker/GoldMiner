#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Props;
using TerrainGeneration;

[CustomEditor(typeof(PropCollection))]
public class PropCollectionEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		var propCollection = (PropCollection) target;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Props", EditorStyles.boldLabel);

		if (propCollection.Props != null)
		{
			for (var i = 0; i < propCollection.Props.Count; i++)
			{
				var prop = propCollection.Props[i];
				if (prop == null) continue;
				EditorGUILayout.LabelField($"Prop {i + 1}: {prop.name}", EditorStyles.boldLabel);

				prop.StaticObject = EditorGUILayout.Toggle("Static Object", prop.StaticObject);
				prop.Spawn = EditorGUILayout.Toggle("Spawn", prop.Spawn);
				prop.Prefab =
					(GameObject) EditorGUILayout.ObjectField("Prefab", prop.Prefab, typeof(GameObject), false);
				prop.NumSamplesBeforeRejection = EditorGUILayout.IntSlider("Num Samples Before Rejection",
					prop.NumSamplesBeforeRejection, 0, 40);
				prop.MaxQuantityPer100M =
					EditorGUILayout.IntSlider("Max Quantity Per 100M", prop.MaxQuantityPer100M, 0, 2500);
				prop.FlatnessTolerance = EditorGUILayout.Slider("Flatness Tolerance", prop.FlatnessTolerance, -1f, 1f);
				prop.Radius = EditorGUILayout.Slider("Radius", prop.Radius, 0.1f, 10f);
				prop.OverrideRideRadius = EditorGUILayout.Toggle("Override Ride Radius", prop.OverrideRideRadius);
				prop.InBoundryOnly = EditorGUILayout.Toggle("In Boundry Only", prop.InBoundryOnly);

				EditorGUILayout.Space();
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(propCollection);
		}
	}
}
#endif