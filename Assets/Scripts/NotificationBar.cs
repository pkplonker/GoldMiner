using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UI;
using UnityEngine;

public class NotificationBar : GenericUnitySingleton<NotificationBar>
{
	private NotificationUI ui;
	
	public void RequestText(string text)
	{
		if (ui == null) GetUI();
		if (ui == null) return;
		ui.UpdateText(text);
	}

	public void ClearText() => RequestText("");
	private void GetUI() => ui = FindObjectOfType<NotificationUI>();
}