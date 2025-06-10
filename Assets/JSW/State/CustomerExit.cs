using UnityEngine;

public class CustomerExit : CustomerState
{
    private CustomerAI ai;
    private UnityEngine.AI.NavMeshAgent agent;
    private bool isExiting = false;

    public override void EnterState(CustomerStateMachine machine)
    {
        base.EnterState(machine);

        // 좌석 비우기
        ai = GetComponent<CustomerAI>();
        foreach (var seat in ai.mySeats)
            seat.Vacate();
        ai.mySeats.Clear();
        ai.isSeated = false;

        // 걷기 상태로 전이 후 ExitPos로 이동, 도착 시 삭제
        var walkState = machine.walkState;
        if (SeatManager.Instance.exitPoint != null)
        {
            machine.ChangeState(walkState);
            walkState.WalkTo(SeatManager.Instance.exitPoint.position, () => {
                Destroy(ai.gameObject);
            });
        }
        else
        {
            Destroy(ai.gameObject);
        }
    }

    public override void StateUpdate()
    {
        if (isExiting && agent.enabled && !agent.pathPending && agent.remainingDistance < 0.3f)
        {
            GameObject.Destroy(ai.gameObject);
        }
    }
}