using UnityEngine;

public class StateMachine_LYJ<T>
{
    private State_LYJ<T> currentState;
    public State_LYJ<T> CurrentState => currentState;

    private T owner;

    public StateMachine_LYJ(T owner)
    {
        this.owner = owner;
    }

    public void ChangeState(State_LYJ<T> targetState)
    {
        currentState?.Exit();
        currentState = targetState;
        currentState.Enter(owner);
    }

    public void Run()
    {
        currentState?.Run();
    }
}