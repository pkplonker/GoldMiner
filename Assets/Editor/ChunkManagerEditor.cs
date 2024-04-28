using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(ChunkManager))]
[CanEditMultipleObjects]
public class MarchingChunkManagerEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var manager = (ChunkManager) target;
	
		if (GUILayout.Button("Generate"))
		{
			manager.ClearChunks();
			manager.GenerateChunks();
		}

		if (GUILayout.Button("Clear"))
		{
			manager.ClearChunks();
		}
	}
}