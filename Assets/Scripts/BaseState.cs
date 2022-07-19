using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
	protected StateMachine stateMachine;

	public virtual void EnterState(StateMachine sm)
	{
		stateMachine = sm;
	}
	
	public void ExitState()
	{
		VirtualStateExit();
		stateMachine = null;
	}

	protected abstract void VirtualStateExit();

	public abstract void Tick();
}