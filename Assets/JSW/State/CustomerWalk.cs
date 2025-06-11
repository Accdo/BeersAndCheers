using UnityEngine;
using UnityEngine.AI;
using System;

public class CustomerWalk : CustomerState
{
    private Vector3 targetPosition;
    private Action onArriveCallback;
    private bool isWalking = false;

    public CustomerWalk(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent) : base(_ai, _animName, _stateMachine, _agent)
    {
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

    public override void EnterState( )
    {
        base.EnterState();
        // CustomerAI에 저장된 목적지와 콜백을 사용
        WalkTo(ai.nextDestination, ai.onArriveCallback);
    }

    public override void StateUpdate()
    {
        if (isWalking && agent.enabled && !agent.pathPending)
        {
            float dist = Vector3.Distance(ai.transform.position, targetPosition);
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