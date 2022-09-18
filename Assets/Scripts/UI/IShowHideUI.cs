//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using StuartHeathTools;
using UnityEngine;

namespace UI
{
	/// <summary>
	///IShowHideUI full description
	/// </summary>
	public interface IShowHideUI
	{
		public abstract void Show();
		public abstract void Hide();

		public abstract void Disable();

		public abstract void Enable();
	}
}