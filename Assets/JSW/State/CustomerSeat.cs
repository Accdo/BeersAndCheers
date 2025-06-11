using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerSeat : CustomerState
{

    public CustomerSeat(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent) : base(_ai, _animName, _stateMachine, _agent)
    {
    }



    public override void EnterState()
    {
        base.EnterState();
        ai.ShowOrderBubble();
        // 착석 처리
        if (ai.mySeats.Count > 0)
        {
            Vector3 sitPos = ai.mySeats[0].SitPoint.position;
            if (UnityEngine.AI.NavMesh.SamplePosition(sitPos, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
                sitPos = hit.position;
            ai.transform.position = sitPos;
            ai.transform.rotation = ai.mySeats[0].SitPoint.rotation;
            ai.isSeated = true;
            // 매뉴받고 식사 끝나면 퇴장
            FoodCompleted();
        }
    }

    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(10f); // 10초 후 퇴장
        stateMachine.ChangeState(ai.exitState);
    }

    public void FoodCompleted()
    {
        // 식사 완료 후 퇴장 상태로 전이
        ai.StartCoroutine(WaitAndExit());
    }


    // UI 버튼 등에서 호출
    public void OnOrderCompleted()
    {
        ai.RequestExit();
    }


}
