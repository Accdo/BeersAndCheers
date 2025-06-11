using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class CustomerAI : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int teamSize = 1;
    [HideInInspector] public List<Seat> mySeats = new List<Seat>();
    [HideInInspector] public bool isSeated = false;
    [HideInInspector] public int queueIndex = -1;
    public Animator anim;
    private CustomerStateMachine stateMachine;
    public Vector3 nextDestination;
    public Action onArriveCallback;

    #region 주문 UI
    public GameObject orderBubblePrefab;
    #endregion

    #region State
    public CustomerWaiting waitingState { get; private set; }
    public CustomerWalk walkState { get; private set; }
    public CustomerSeat seatState { get; private set; }
    public CustomerExit exitState { get; private set; }
    #endregion

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<CustomerStateMachine>();
        anim = GetComponent<Animator>();

        //스테이트
        waitingState = new CustomerWaiting(this, "Wait", stateMachine, agent);
        walkState = new CustomerWalk(this, "Walk", stateMachine, agent);
        seatState = new CustomerSeat(this, "Sit", stateMachine, agent);
        exitState = new CustomerExit(this, "Walk", stateMachine, agent);
    }

    void Start()
    {
        stateMachine.ChangeState(waitingState);
        HideOrderBubble();
    }

    private void Update()
    {
        stateMachine.currentState?.StateUpdate();
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
                stateMachine.ChangeState(seatState);
            };
            stateMachine.ChangeState(walkState);
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
    public void ShowOrderBubble()
    {
        if (orderBubblePrefab != null)
            orderBubblePrefab.SetActive(true);
    }
    public void HideOrderBubble()
    {
        if (orderBubblePrefab != null)
            orderBubblePrefab.SetActive(false);
    }

    public void RequestExit()
    {
        stateMachine.ChangeState(exitState);
    }

    public void DestroyCustomer()
    {
        Destroy(this.gameObject);
    }
}