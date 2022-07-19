using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
	protected BaseState currentState;

	protected virtual void Update()
	{
		currentState.Tick();
	}

	protected virtual void ChangeState(BaseState state)
	{
		currentState?.ExitState();
		currentState = state;
		currentState.EnterState(this);
	}
}