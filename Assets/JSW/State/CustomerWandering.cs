// /State/CustomerWandering.cs
using UnityEngine;
using UnityEngine.AI;

public class CustomerWandering : CustomerState
{
    private float wanderTimer; // 총 배회 시간 타이머
    private float maxWanderTime; // 최대 배회 시간

    private float idleTimer; // 한 지점에서 머무는 시간 타이머
    private float maxIdleTime; // 한 지점에서 머무는 최대 시간

    public CustomerWandering(CustomerAI customer, string animBoolName, CustomerStateMachine stateMachine, NavMeshAgent agent)
        : base(customer, animBoolName, stateMachine, agent)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        // 배회 시간을 랜덤하게 설정하여 손님마다 다르게 행동하도록 함
        maxWanderTime = Random.Range(15f, 50f);
        wanderTimer = 0f;
        idleTimer = 0f;

        FindNewDestination();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        wanderTimer += Time.deltaTime;

        // 총 배회 시간이 다 되면, 가게에 들어갈 수 있는지 확인
        if (wanderTimer >= maxWanderTime)
        {
            // 1. 문이 열려있는지 확인
            bool isDoorOpen = (CustomerSpawnManager.Instance.door == null || CustomerSpawnManager.Instance.door.GetDoorState());

            if (isDoorOpen)
            {
                // 2. 문이 열려있으면, SeatManager에 자리가 있는지 확인
                if (SeatManager.Instance.CanAcceptNewCustomer(ai.teamSize))
                {
                    // 자리가 있으면 대기열로 이동하고 상태 변경
                    int queueIndex = SeatManager.Instance.waitingQueue.Count;
                    ai.MoveToQueuePoint(queueIndex);
                    stateMachine.ChangeState(ai.waitingState);
                }
                else
                {
                    // 자리가 없으면 그냥 떠나게 한다.
                    stateMachine.ChangeState(ai.exitState);
                }
            }
            else
            {
                // 3. 문이 닫혀있으면 그냥 떠나게 한다.
                stateMachine.ChangeState(ai.exitState);
            }
            return;
        }

        // 목표 지점에 도착했는지 확인
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            idleTimer += Time.deltaTime;
            // 한 지점에서 잠시 머무른 후 다음 목적지로 이동
            if (idleTimer >= maxIdleTime)
            {
                FindNewDestination();
            }
        }
    }

    private void FindNewDestination()
    {
        idleTimer = 0f;
        maxIdleTime = Random.Range(0f, 3f);
        Vector3 newDest = WanderPointManager.Instance.GetRandomWanderPoint();
        agent.SetDestination(newDest);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}