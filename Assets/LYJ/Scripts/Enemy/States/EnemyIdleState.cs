using UnityEngine;

public class EnemyIdleState : State_LYJ<Enemy>
{
    private Enemy owner;
    private float idleStartTime;
    private float idleTime;
    public override void Enter(Enemy owner)
    {
        base.Enter(owner);
        this.owner = owner;
        if (owner.NavMeshAgent != null)
        {
            if (owner.NavMeshAgent.isOnNavMesh)
            {
                owner.NavMeshAgent.isStopped = true;
            }
        }
        owner.Anim.SetBool("Idle", true);
        idleTime = Random.Range(3f, 5f);
        idleStartTime = Time.time;

    }

    public override void Run()
    {
        base.Run();
        if (owner.ChaseTarget != null)
        {
            if (Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) > owner.Data.AttackRange * 1.5f)
            {
                owner.ChangeState(EnemyStates.Move);
                return;
            }
        }
        if (Time.time - idleStartTime >= idleTime)
        {
            owner.ChangeState(EnemyStates.Stray);
            return;
        }
    }

    public override void Exit()
    {
        owner.Anim.SetBool("Idle", false);
        base.Exit();
    }
}
