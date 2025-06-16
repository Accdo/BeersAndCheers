using UnityEngine;

public class EnemyIdleState : State_LYJ<Enemy>
{
    private Enemy owner;
    public override void Enter(Enemy owner)
    {
        base.Enter(owner);
        this.owner = owner;
    }

    public override void Run()
    {
        base.Run();
        if (owner.Target != null)
        {
            if (Vector3.Distance(owner.transform.position, owner.Target.transform.position) > owner.Data.AttackRange)
            {
                owner.ChangeState(EnemyStates.Move);
                return;
            }
            if (Vector3.Distance(owner.transform.position, owner.Target.transform.position) <= owner.Data.AttackRange)
            {
                owner.ChangeState(EnemyStates.Attack);
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
