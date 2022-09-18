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
	[SerializeField] private GameObject _truckUIPrefab;

	public string GetInteractMessage() => _interactText;


	public void Interact(PlayerInteractionStateMachine player)
	{
		OpenTruckUI(player);
		player.GetComponent<PlayerMovement>().SetCanMove(false);
	}

	private void OpenTruckUI(PlayerInteractionStateMachine player)
	{
		if (_truckUI == null) CreateTruckUI();
		_truckUI.Show(player);
	}

	private void CreateTruckUI() => _truckUI = Instantiate(_truckUIPrefab).GetComponent<TruckUI>();
}