using UnityEngine;
using UnityEngine.AI;
using System;

public class CustomerWalk : CustomerState
{
    private CustomerAI ai;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private Action onArriveCallback;
    private bool isWalking = false;

    void Awake()
    {
        ai = GetComponent<CustomerAI>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void WalkTo(Vector3 destination, Action onArrive = null)
    {
        targetPosition = destination;
        onArriveCallback = onArrive;
        isWalking = true;
        agent.enabled = true;
        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(destination);
    }

    public override void EnterState(CustomerStateMachine machine)
    {
        base.EnterState(machine);
        // CustomerAI에 저장된 목적지와 콜백을 사용
        var ai = GetComponent<CustomerAI>();
        WalkTo(ai.nextDestination, ai.onArriveCallback);
    }

    public override void StateUpdate()
    {
        if (isWalking && agent.enabled && !agent.pathPending)
        {
            float dist = Vector3.Distance(transform.position, targetPosition);
            // remainingDistance가 유효하지 않을 때도 도착 처리
            if ((agent.remainingDistance <= 0.3f && !agent.pathPending) || dist < 0.3f)
            {
                isWalking = false;
                agent.enabled = false;
                onArriveCallback?.Invoke();
                onArriveCallback = null;
            }
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        isWalking = false;
        agent.enabled = false;
        onArriveCallback = null;
    }
} 