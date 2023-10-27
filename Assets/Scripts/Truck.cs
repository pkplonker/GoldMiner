using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class Truck : MonoBehaviour, IInteractable
{
	[SerializeField] private string InteractText = "Click to pickup";
	private bool isActiveTarget;
	private TruckUI truckUI;
	public string GetInteractMessage() => InteractText;

	private void Awake()
	{
		truckUI = FindObjectOfType<TruckUI>();
	}

	public void Interact(PlayerInteractionStateMachine player)
	{
		OpenTruckUI(player);
	}

	private void OpenTruckUI(PlayerInteractionStateMachine player)
	{
		ServiceLocator.Instance.GetService<CanvasGroupController>().Show(truckUI);
		truckUI.Init(player);
	}

	public void Close() => ServiceLocator.Instance.GetService<CanvasGroupController>().Hide(truckUI);

}