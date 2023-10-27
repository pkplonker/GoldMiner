using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatUIController : MonoBehaviour
{
	public RectTransform commandsContainer;
	public GameObject commandButtonPrefab;
	[SerializeField] private CanvasGroup canvasGroup;
	private CheatConsole cheatConsole;
	public static event Action<bool> CheatConsoleActive;

	private void Start()
	{
		Hide();
		cheatConsole = CheatConsole.Instance;
		if (cheatConsole == null)
		{
			Debug.LogError("CheatConsole not found in the scene!");
			return;
		}

		PopulateCommandsList();
		cheatConsole.OnShowConsoleChanged += Toggle;
	}

	private void Toggle()
	{
		if (canvasGroup.alpha == 0) Show();
		else Hide();
	}

	private void Hide()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		CheatConsoleActive?.Invoke(false);
	}

	private void Show()
	{
		PopulateCommandsList();
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		CheatConsoleActive?.Invoke(true);
	}

	private List<Button> buttons = new List<Button>();

	private void PopulateCommandsList()
	{
		ClearButtons();
		foreach (var command in cheatConsole.GetRegisteredCommands())
		{
			var button = Instantiate(commandButtonPrefab, commandsContainer).GetComponent<Button>();
			buttons.Add(button);
			button.GetComponentInChildren<TextMeshProUGUI>().text = command;
			button.onClick.AddListener(() => cheatConsole.ExecuteCommand(command));
		}
	}

	private void ClearButtons()
	{
		foreach (var b in buttons)
		{
			Destroy(b.gameObject);
		}

		buttons.Clear();
	}
}