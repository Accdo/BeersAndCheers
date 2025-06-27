using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CustomerSeat : CustomerState
{
    private const float SPECIAL_REQUEST_CHANCE = 2.5f; // 5% 확률로 특별 요청

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

            // 퀘스트 손님인 경우
            if (ai.isQuestCustomer)
            {
                // 자리에 앉는 순간, 퀘스트 제안을 딱 한 번 시도
                ai.TryOfferQuest();
                return; // 퀘스트 손님은 일반/특별 주문을 하지 않으므로 여기서 종료
            }

            // 주문 받기 or 특별 요청 (퀘스트 손님이 아닐 경우에만 실행됨)
            float per = Random.Range(0, 100);
            if(per < SPECIAL_REQUEST_CHANCE)
            {
                // 특별 요청 상태로 설정
                ai.hasSpecialRequest = true;
                ai.ShowSpecialRequestImage();
            }
            else
            {
                // 일반 주문 시작
                ai.StartOrdering();
            }
        }
    }

    public void FoodCompleted()
    {
        // 식사 완료 후 퇴장 상태로 전이
        ai.CustormerExit();
    }

    public void OnOrderCompleted()
    {
        ai.CustormerExit();
    }
}
