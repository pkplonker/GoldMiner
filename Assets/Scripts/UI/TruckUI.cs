//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using StuartHeathTools;

namespace UI
{
	/// <summary>
	///TruckUI full description
	/// </summary>
	public class TruckUI : CanvasGroupBase
	{
		private void Awake() => Hide();

		public void Show() => ShowUI();
		public void Hide() => HideUI();
	}
}