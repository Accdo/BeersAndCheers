using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class EnemyMoveState : State_LYJ<Enemy>
{
    private Enemy owner;
    public override void Enter(Enemy owner)
    {
        base.Enter(owner);
        this.owner = owner;
        owner.NavMeshAgent.isStopped = false;
    }

    public override void Run()
    {
        base.Run();
        if (owner.Target != null && Vector3.Distance(transform.position, owner.Target.transform.position) <= owner.Data.AttackRange)
        {
            owner.ChangeState(EnemyStates.Attack);
            return;
        }
        if (!owner.Target)
        {
            owner.NavMeshAgent.SetDestination(owner.FirstPos);
        }
        if (owner.Target != null && Vector3.Distance(owner.transform.position, owner.Target.transform.position) > owner.Data.AttackRange)
        {
            owner.NavMeshAgent.SetDestination(owner.Target.transform.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
