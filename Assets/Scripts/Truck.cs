using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class Truck : MonoBehaviour, IInteractable
{
	[SerializeField] private string _interactText = "Click to pickup";
	private bool _isActiveTarget;
	private TruckUI _truckUI;
	public string GetInteractMessage() => _interactText;

	private void Awake()
	{
		_truckUI = FindObjectOfType<TruckUI>();
	}

	public void Interact(PlayerInteractionStateMachine player)
	{
		OpenTruckUI(player);
	}

	private void OpenTruckUI(PlayerInteractionStateMachine player)
	{
		CanvasGroupController.Instance.Show(_truckUI);
		_truckUI.Init(player);
	}

	public void Close() => CanvasGroupController.Instance.Hide(_truckUI);

}