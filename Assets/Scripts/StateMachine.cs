using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
	protected BaseState CurrentState;

	protected virtual void Update()
	{
		CurrentState.Tick();
	}

	protected virtual void ChangeState(BaseState state)
	{
		CurrentState?.ExitState();
		CurrentState = state;
		CurrentState.EnterState(this);
	}
}