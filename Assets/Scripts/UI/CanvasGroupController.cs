using System;
using System.Collections.Generic;
using Player;
using StuartHeathTools;
using Targets;
using UnityEngine;

namespace UI
{
	public class CanvasGroupController : GenericUnitySingleton<CanvasGroupController>
	{
		private List<IShowHideUI> uis = new ();
		public NewItemPickupUI NewItemPickupUI { get; private set; }
		public TruckUI TruckUI { get; private set; }

		private void Start()
		{
			NewItemPickupUI = GetComponentInChildren<NewItemPickupUI>();
			TruckUI= GetComponentInChildren<TruckUI>();
		}

		public void Show(IShowHideUI ui)
		{
			if (uis.Contains(ui))
			{
				uis.RemoveAt(uis.IndexOf(ui));
				uis.Add(ui);
			}

			uis.RemoveAll(x => x == null);
			ui.Toggle();
			for (var i = 0; i < uis.Count-1; i++)
			{
				ui.Disable();
			}
		}

		public void Hide(IShowHideUI ui)
		{
			if (uis.Contains(ui))
			{
				if (uis.IndexOf(ui) != uis.Count - 1) return;
				uis.Remove(ui);
				if (uis.Count > 0)
				{
					uis[^1].Enable();
				}
			}
			else
			{
				ui.Hide();
			}
		}
	}
}