using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(WorldGenerator))]
[CanEditMultipleObjects]
public class WorldGeneratorEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var manager = (WorldGenerator) target;
	
		if (GUILayout.Button("Generate"))
		{
			manager.ClearChunks();
			manager.Generate();
		}

		if (GUILayout.Button("Clear"))
		{
			manager.ClearChunks();
		}
	}
}