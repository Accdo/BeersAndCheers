using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CustomerAI : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int teamSize = 1;
    [HideInInspector] public List<Seat> mySeats = new List<Seat>();
    [HideInInspector] public bool isSeated = false;
    [HideInInspector] public int queueIndex = -1;
    private CustomerStateMachine stateMachine;
    public Vector3 nextDestination;
    public System.Action onArriveCallback;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<CustomerStateMachine>();
    }

    // 좌석 배정 시 상태머신에서 호출
    public void AssignSeats(List<Seat> seats)
    {
        mySeats = seats;
        if (mySeats.Count > 0)
        {
            nextDestination = mySeats[0].SitPoint.position;
            onArriveCallback = () =>
            {
                // 도착 후 착석 상태로 전이
                GetComponent<CustomerStateMachine>().ChangeState(GetComponent<CustomerSeat>());
            };
            GetComponent<CustomerStateMachine>().ChangeState(GetComponent<CustomerWalk>());
        }
    }

    // 대기열 위치로 이동
    public void MoveToQueuePoint(int index)
    {
        var queuePoints = SeatManager.Instance.queuePoints;
        if (index >= 0 && index < queuePoints.Count)
        {
            var point = queuePoints[index];
            agent.SetDestination(point.position);
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] queueIndex {index} is out of range! queuePoints.Count={queuePoints.Count}");
            // 대기 위치가 부족할 때는 마지막 위치로 이동하거나, 적당한 대체 위치로 이동
            if (queuePoints.Count > 0)
                agent.SetDestination(queuePoints[queuePoints.Count - 1].position);
        }
    }
}