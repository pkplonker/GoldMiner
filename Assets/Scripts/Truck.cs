using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class Truck : MonoBehaviour, IInteractable
{
	[SerializeField] private string _interactText = "Click to pickup";
	private bool _isActiveTarget;

	public string GetInteractMessage() => _interactText;


	public void Interact(PlayerInteractionStateMachine player)
	{
		OpenTruckUI(player);
	}

	private void OpenTruckUI(PlayerInteractionStateMachine player)
	{
		CanvasGroupController.Instance.ShowTruckUI(player);
		player.GetComponent<PlayerMovement>().SetCanMove(false);
	}

	public void Close() => CanvasGroupController.Instance.HideTruckUI();

}