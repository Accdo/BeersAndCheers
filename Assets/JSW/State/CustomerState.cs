using UnityEngine;
using UnityEngine.AI;

public class CustomerState
{
    protected CustomerStateMachine stateMachine;
    protected CustomerAI ai;
    protected NavMeshAgent agent;
    protected string animName;

    public CustomerState(CustomerAI _ai, string _animName, CustomerStateMachine _stateMachine, NavMeshAgent _agent)
    {
        ai = _ai;
        animName = _animName;
        stateMachine = _stateMachine;
        agent = _agent;
    }
    // 상태 진입 시 호출
    public virtual void EnterState()
    {
        ai.anim.SetBool(animName, true);
    }

    // 상태 종료 시 호출
    public virtual void ExitState()
    {
        ai.anim.SetBool(animName, false);
    }

    public virtual void StateUpdate() { }
    
}