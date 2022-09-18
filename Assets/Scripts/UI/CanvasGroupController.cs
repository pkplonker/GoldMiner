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

		public void Show(IShowHideUI ui)
		{
			ui.Show();
		}

		public void Hide(IShowHideUI ui)
		{
			ui.Hide();
		}
	}
}