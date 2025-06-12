using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class CustomerAI : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int teamSize = 1;
    public CustomerGroup myGroup;
    public List<Seat> mySeats = new List<Seat>();
    public bool isSeated = false;
    public int queueIndex = -1;
    public Animator anim;
    private CustomerStateMachine stateMachine;
    public Vector3 nextDestination;
    public Action onArriveCallback;

    #region 만족도
    public float satisfactionScore = 100;
    public float waitingTime = 0f; // 대기 시간
    public float maxWaitingTime = 60f; // 최대 대기 시간
    #endregion

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

        //음식 받기전 까지 대기 시간 증가
        //
        WatingMenu();
    }

    private void WatingMenu()
    {
        if (waitingTime <= maxWaitingTime)
        {
            waitingTime += Time.deltaTime;
            SatisfactionScoreUpDown(waitingTime / 10f);

        }
    }

    #region 이동
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
    #endregion

    #region Customer UI
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
    #endregion

    #region 만족도
    public void SatisfactionScoreUpDown(float amount)
    {
        satisfactionScore += amount;
        satisfactionScore = Mathf.Clamp(satisfactionScore, 0, 100);
        if (satisfactionScore <= 0)
        {
            // 만족도가 0 이하가 되면 퇴장 상태로 전이
            RequestExit();
        }

    }

    //만족도 결과에 따라 서 결과처리
    public void ResultOfStisfaciton()
    {
        if (satisfactionScore >= 90)
        {
            //팁주기 로직
            // ex) 20% 팁 주기

        }
        else if (satisfactionScore >= 70)
        {
            //팁주기 로직
            // ex) 10% 팁 주기
        }
        else if (satisfactionScore >= 30)
        {
            // 음식가격만 지불하기

        }
        else
        {
            //디메리트 주기
            // ex) 음식 가격 안주기

        }

    }
    #endregion

    public void RequestExit()
    {
        stateMachine.ChangeState(exitState);
    }

    public void DestroyCustomer(int time = 0)
    {
        Destroy(this.gameObject, time);
    }

   


}