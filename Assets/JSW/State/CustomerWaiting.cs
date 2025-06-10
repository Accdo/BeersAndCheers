using UnityEngine;

public class CustomerWaiting : CustomerState
{
    private CustomerAI ai;

    void Awake()
    {
        ai = GetComponent<CustomerAI>();
    }

    public override void EnterState(CustomerStateMachine machine)
{
    base.EnterState(machine);
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
        // else 아무것도 하지 않음 (경고도 출력하지 않음)
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
        // else 아무것도 하지 않음 (경고도 출력하지 않음)
    }
}

    public override void StateUpdate()
    {
        // 좌석 배정은 SeatManager에서 AssignSeats 호출로 처리됨
    }
}