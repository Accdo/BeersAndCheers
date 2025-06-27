using System.Collections;
using UnityEngine;

public class EnemyAttackState : State_LYJ<Enemy>
{
    private Enemy owner;
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
    }

    public override void Run()
    {
        base.Run();
        if (owner.CanAttack && (!owner.ChaseTarget || Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) > owner.Data.AttackRange * 1.5f))
        {
            owner.ChangeState(EnemyStates.Move);
            return;
        }
        if (owner.CanAttack && owner.ChaseTarget && Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) <= owner.Data.AttackRange * 1.5f)
        {
            StartCoroutine(AttackCoroutine());
            return;
        }
    }

    IEnumerator AttackCoroutine()
    {
        owner.CanAttack = false;
        owner.Anim.SetTrigger("Attack");
        yield return new WaitForSeconds(owner.Data.AttackFirstDelay);

        Vector3 centerPoint = owner.transform.position + (owner.transform.forward * (owner.Data.AttackRange *1.5f)) + (owner.transform.up * owner.Data.AttackHeight);
        Collider[] hitTargets = Physics.OverlapSphere(centerPoint, owner.Data.AttackRange, owner.HitTarget);

        foreach (var target in hitTargets)
        {
            var player = target.GetComponent<Player_LYJ>();
            if (player != null)
            {
                player.Damage(owner.Data.AttackPower);
            }
        }

        yield return new WaitForSeconds(owner.Data.AttackTotalDelay - owner.Data.AttackFirstDelay);

        owner.CanAttack = true;
        if (owner.ChaseTarget != null && Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) <= owner.Data.AttackRange * 1.5f)
        {
        }
        else
        {
            owner.ChangeState(EnemyStates.Move);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
