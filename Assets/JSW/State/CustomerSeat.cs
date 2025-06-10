using UnityEngine;

public class CustomerSeat : CustomerState
{
    private CustomerAI ai;

    void Awake()
    {
        ai = GetComponent<CustomerAI>();
    }

    public override void EnterState(CustomerStateMachine machine)
    {
        base.EnterState(machine);
        Debug.Log($"[CustomerSeat] EnterState for {ai.name}, mySeats count: {ai.mySeats.Count}");
        // 착석 처리
        if (ai.mySeats.Count > 0)
        {
            Vector3 sitPos = ai.mySeats[0].SitPoint.position;
            if (UnityEngine.AI.NavMesh.SamplePosition(sitPos, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
                sitPos = hit.position;
            ai.transform.position = sitPos;
            ai.transform.rotation = ai.mySeats[0].SitPoint.rotation;
            ai.isSeated = true;
            // 일정 시간 후 퇴장 상태로 전이
            ai.StartCoroutine(WaitAndExit());
        }
    }

    private System.Collections.IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(10f); // 10초 후 퇴장
        stateMachine.ChangeState(GetComponent<CustomerExit>());
    }
}