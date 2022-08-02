using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
	protected StateMachine StateMachine;

	public virtual void EnterState(StateMachine sm)
	{
		StateMachine = sm;
	}
	
	public void ExitState()
	{
		VirtualStateExit();
		StateMachine = null;
	}

	protected abstract void VirtualStateExit();

	public abstract void Tick();
}