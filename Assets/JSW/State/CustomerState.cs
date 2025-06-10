using UnityEngine;

public abstract class CustomerState : MonoBehaviour
{
    protected CustomerStateMachine stateMachine;

    // 상태 진입 시 호출
    public virtual void EnterState(CustomerStateMachine machine)
    {
        stateMachine = machine;
        this.enabled = true;
    }

    // 상태 종료 시 호출
    public virtual void ExitState()
    {
        this.enabled = false;
    }

    // 상태별 매 프레임 처리
    public virtual void StateUpdate() { }
    
    // 필요시 공통 메서드 추가 가능
    // 예: protected void MoveTo(Vector3 pos) { ... }
}