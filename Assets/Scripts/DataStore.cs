using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DataStore<T> : ScriptableObject
{
	[SerializeField] private T currentValue;
	[SerializeField] private T startValue;

	private T originalValue;

	[SerializeField] internal bool wipeOnLoad = true;

	public T Value
	{
		get => currentValue;
		set => this.currentValue = value;
	}

	private T StartValue
	{
		get => startValue;
		set => this.startValue = value;
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += LogPlayModeState;
#endif
	}

	private void OnDisable()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged -= LogPlayModeState;
#endif
	}

#if UNITY_EDITOR
	private void LogPlayModeState(PlayModeStateChange state)
	{
		if (wipeOnLoad && state == PlayModeStateChange.EnteredPlayMode)
		{
			currentValue = startValue;
		}
	}
#endif
}

[CreateAssetMenu(fileName = "IntData", menuName = "Data Stores/Int Data", order = 0)]
public class IntData : DataStore<int> { }

[CreateAssetMenu(fileName = "FloatData", menuName = "Data Stores/Float Data", order = 1)]
public class FloatData : DataStore<float> { }

[CreateAssetMenu(fileName = "StringData", menuName = "Data Stores/String Data", order = 2)]
public class StringData : DataStore<float> { }