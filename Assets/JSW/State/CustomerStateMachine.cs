using UnityEngine;

public class CustomerStateMachine : MonoBehaviour
{
    public CustomerState currentState { get; private set; }

    // 각 상태 인스턴스 참조
    [HideInInspector] public CustomerWaiting waitingState;
    [HideInInspector] public CustomerSeat seatState;
    [HideInInspector] public CustomerExit exitState;
    [HideInInspector] public CustomerWalk walkState;

    void Awake()
    {
        waitingState = GetComponent<CustomerWaiting>();
        seatState = GetComponent<CustomerSeat>();
        exitState = GetComponent<CustomerExit>();
        walkState = GetComponent<CustomerWalk>();
    }

    void Start()
    {
        ChangeState(waitingState);
    }

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
            currentState.EnterState(this);
    }
}