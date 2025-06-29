using UnityEngine;
using UnityEngine.AI;

public class CustomerWaiting : CustomerState
{
    public CustomerWaiting(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent) : base(_ai, _animName, _stateMachine, _agent)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        // 대기열에 이미 있으면 queueIndex만 갱신 (MoveToQueuePoint 호출하지 않음)
        if (SeatManager.Instance.waitingQueue.Contains(ai))
        {
            int idx = SeatManager.Instance.waitingQueue.IndexOf(ai);
            ai.queueIndex = idx;
            // queueIndex가 유효할 때만 이동
            if (idx >= 0 && idx < SeatManager.Instance.queuePoints.Count)
            {
                ai.MoveToQueuePoint(idx);
            }
        }
        else
        {
            // 대기열에 없으면 새로 추가
            int idx = SeatManager.Instance.AddToQueue(ai);
            if (idx >= 0 && idx < SeatManager.Instance.queuePoints.Count)
            {
                ai.queueIndex = idx;
                ai.MoveToQueuePoint(idx);
            }
        }
    }

    public override void StateUpdate()
    {
        // 좌석 배정은 SeatManager에서 AssignSeats 호출로 처리됨
        Door door = CustomerSpawnManager.Instance.door;
        WoodenSign woodenSign = WoodenSign.instance;
        if (door != null && !door.GetDoorState() || !woodenSign.isOpen)
        {
            stateMachine.ChangeState(ai.exitState);
        }

    }
}