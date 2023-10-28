using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IService
{
	public bool IsSurvival = true;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		ServiceLocator.Instance.RegisterService(this);
	}

	public void Initialize() { }
}