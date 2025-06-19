using System.Collections;
using UnityEngine;

public class EnemyAttackState : State_LYJ<Enemy>
{
    private Enemy owner;
    public override void Enter(Enemy owner)
    {
        base.Enter(owner);
        this.owner = owner;
        owner.NavMeshAgent.isStopped = true;
    }

    public override void Run()
    {
        base.Run();
        if (!owner.Target || Vector3.Distance(owner.transform.position, owner.Target.transform.position) > owner.Data.AttackRange)
        {
            owner.ChangeState(EnemyStates.Move);
            return;
        }
        if (owner.CanAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        owner.CanAttack = false;

        if (owner.Target is Player_LYJ player)
        {
            player.Damage(owner.Data.AttackPower);
        }

        yield return new WaitForSeconds(owner.Data.AttackDelay);

        owner.CanAttack = true;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
