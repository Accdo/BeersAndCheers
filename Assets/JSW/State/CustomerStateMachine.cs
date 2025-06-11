using UnityEngine;

public class CustomerStateMachine : MonoBehaviour
{
    public CustomerState currentState { get; private set; }


    void Update()
    {
        if (currentState != null)
            currentState.StateUpdate();
    }

    public void ChangeState(CustomerState newState)
    {
        if (currentState != null)
            currentState.ExitState();

        currentState = newState;
        if (currentState != null)
            currentState.EnterState();
    }
}