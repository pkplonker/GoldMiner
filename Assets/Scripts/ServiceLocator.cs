//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using StuartHeathTools;
using UnityEngine;

/// <summary>
///ServiceLocator full description
/// </summary>
public class ServiceLocator : GenericUnitySingleton<ServiceLocator>
{
	private static Dictionary<System.Type, IService> services = new();

	public void RegisterService<T>(T service) where T : IService
	{
		var type = typeof(T);
		if (!services.ContainsKey(type))
		{
			services[type] = service;
			service.Initialize();
		}
		else
		{
			Debug.LogWarning($"Service {type} is already registered");
		}
	}

	public T GetService<T>() where T : IService
	{
		var type = typeof(T);
		if (services.TryGetValue(type, out var service))
		{
			return (T) service;
		}

		Debug.LogError($"Service {type} not found");
		return default;
	}
}

public interface IService
{
	void Initialize();
}