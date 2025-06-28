using UnityEngine;

public class AnimalIdleState : State_LYJ<Animal>
{
    private Animal owner;
    private float idleStartTime;
    private float idleTime;
    public override void Enter(Animal owner)
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
        idleTime = Random.Range(3f, 7f);
        idleStartTime = Time.time;
    }

    public override void Run()
    {
        base.Run();
        if (Time.time - idleStartTime >= idleTime)
        {
            owner.ChangeState(AnimalStates.Stray);
            return;
        }
    }

    public override void Exit()
    {
        owner.Anim.SetBool("Idle", false);
        base.Exit();
    }
}
