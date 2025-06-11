using UnityEngine;
using UnityEngine.AI;

public class CustomerExit : CustomerState
{

    private bool isExiting = false;

    public CustomerExit(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent) : base(_ai, _animName, _stateMachine, _agent)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        // 좌석 비우기
        foreach (var seat in ai.mySeats)
            seat.Vacate();
        ai.mySeats.Clear();
        ai.isSeated = false;

        if (SeatManager.Instance.exitPoint != null)
        {
            // 걷기 애니메이션 재생
            ai.agent.enabled = true;
            ai.agent.SetDestination(SeatManager.Instance.exitPoint.position);
            isExiting = true;
        }
        else
        {
            ai.DestroyCustomer();
        }
    }

    public override void StateUpdate()
    {
        if (isExiting && ai.agent.enabled && !ai.agent.pathPending && ai.agent.remainingDistance < 0.3f)
        {
            ai.DestroyCustomer();
            isExiting = false;
        }
    }
}