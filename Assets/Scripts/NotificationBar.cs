using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UI;
using UnityEngine;

public class NotificationBar : GenericUnitySingleton<NotificationBar>
{
	private NotificationUI _ui;
	
	public void RequestText(string text)
	{
		if (_ui == null) GetUI();
		if (_ui == null) return;
		_ui.UpdateText(text);
	}

	public void ClearText() => RequestText("");
	private void GetUI() => _ui = FindObjectOfType<NotificationUI>();
}