using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CustomerSeat : CustomerState
{
    public CustomerSeat(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent) : base(_ai, _animName, _stateMachine, _agent)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        // 착석 처리
        if (ai.mySeats.Count > 0)
        {
            Vector3 sitPos = ai.mySeats[0].SitPoint.position;
            if (UnityEngine.AI.NavMesh.SamplePosition(sitPos, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
                sitPos = hit.position;
            ai.transform.position = sitPos;
            ai.transform.rotation = ai.mySeats[0].SitPoint.rotation;
            ai.isSeated = true;
            
            // 주문 받기
            ai.StartOrdering();
        }
    }

    public void FoodCompleted()
    {
        // 식사 완료 후 퇴장 상태로 전이
        ai.StartExiting();
    }

    public void OnOrderCompleted()
    {
        ai.RequestExit();
    }
}
