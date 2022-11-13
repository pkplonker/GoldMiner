using System;
using System.Collections.Generic;
using System.Text;
using Player;
using StuartHeathTools;
using Targets;
using UnityEngine;

namespace UI
{
	public class CanvasGroupController : GenericUnitySingleton<CanvasGroupController>
	{
		private List<IShowHideUI> uis = new List<IShowHideUI>();
		public NewItemPickupUI NewItemPickupUI { get; private set; }
		public TruckUI TruckUI { get; private set; }

		private void Start()
		{
			NewItemPickupUI = GetComponentInChildren<NewItemPickupUI>();
			TruckUI= GetComponentInChildren<TruckUI>();
		}

		public void Show(IShowHideUI ui)
		{
	
		}

	
		public void Hide(IShowHideUI ui)
		{
		
		}
	}
}