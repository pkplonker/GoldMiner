using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CheatConsole : MonoBehaviour
{
	private static CheatConsole instance;
	public static CheatConsole Instance => instance;
	private MethodInfo[] methods;
	private readonly Dictionary<string, Action> commands = new();
	private MethodInfo[] cachedCommands;
	public event Action OnShowConsoleChanged;

	private void Awake()
	{
		if (instance != null && instance != this) Destroy(gameObject);
		else instance = this;

		methods = GetCheatMethods();
		DontDestroyOnLoad(gameObject);
	}

	public void RegisterCommand(string commandName, Action action)
	{
		if (commands.ContainsKey(commandName))
		{
			Debug.LogWarning($"Command {commandName} is already registered. Overwriting...");
			commands[commandName] = action;
		}
		else
		{
			commands.Add(commandName, action);
		}
	}

	private MethodInfo[] GetCheatMethods() =>
		cachedCommands ??= TypeCache.GetMethodsWithAttribute<CheatCommandAttribute>().ToArray();

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			OnShowConsoleChanged?.Invoke();
			// foreach (var method in methods)
			// {
			// 	ExecuteCommand(method.Name);
			// }
			//
			// foreach (var kvp in commands)
			// {
			// 	kvp.Value?.Invoke();
			// }
		}
	}

	public void ExecuteCommand(string command)
	{
		foreach (var kvp in commands.Where(kvp => kvp.Key.Equals(command, StringComparison.OrdinalIgnoreCase)))
		{
			kvp.Value?.Invoke();
			return;
		}

		foreach (var method in methods)
		{
			var attribute = method.GetCustomAttribute<CheatCommandAttribute>();
			if (attribute == null || (!attribute.CommandName.Equals(command, StringComparison.OrdinalIgnoreCase) &&
			                          !method.Name.Equals(command, StringComparison.OrdinalIgnoreCase))) continue;
			try
			{
				if (method.IsStatic) method.Invoke(null, null);
				else method.Invoke(GetInstance(method.DeclaringType), null);
				return;
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to execute cheat command: " + ex);
				return;
			}
		}

		Debug.LogError("Cheat command not found: " + command);
	}

	private object GetInstance(Type methodDeclaringType)
	{
		if (typeof(MonoBehaviour).IsAssignableFrom(methodDeclaringType))
		{
			var component = FindObjectOfType(methodDeclaringType);
			if (component != null)
			{
				return component;
			}
		}

		return null;
	}

	public IEnumerable<string> GetRegisteredCommands() => commands.Keys.Concat(GetCheatMethods().Select(x => x.Name));
}

[AttributeUsage(AttributeTargets.Method)]
public class CheatCommandAttribute : Attribute
{
	public string CommandName { get; private set; }
	public string Description { get; private set; }

	public CheatCommandAttribute(string commandName, string description = "")
	{
		CommandName = commandName;
		Description = description;
	}

	public CheatCommandAttribute()
	{
		CommandName = string.Empty;
		Description = string.Empty;
	}
}