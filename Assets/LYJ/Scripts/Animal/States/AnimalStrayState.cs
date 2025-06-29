using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AnimalStrayState : State_LYJ<Animal>
{
    private const float STRAY_RADIUS = 15f;
    private Animal owner;
    private float strayStartTime;
    private float strayingTime;
    public override void Enter(Animal owner)
    {
        base.Enter(owner);
        this.owner = owner;
        if (owner.NavMeshAgent != null)
        {
            if (owner.NavMeshAgent.isOnNavMesh)
            {
                owner.NavMeshAgent.isStopped = false;
            }
        }
        owner.Anim.SetBool("Walk", true);
        strayingTime = Random.Range(2f, 6f);
        strayStartTime = Time.time;
        SetDestination();
    }

    public override void Run()
    {
        base.Run();
        if (Time.time - strayStartTime >= strayingTime)
        {
            owner.ChangeState(AnimalStates.Idle);
            return;
        }
        if (!owner.NavMeshAgent.pathPending && owner.NavMeshAgent.remainingDistance < 0.5f)
        {
            SetDestination();
        }
    }

    public override void Exit()
    {
        owner.Anim.SetBool("Walk", false);
        base.Exit();
    }

    private void SetDestination()
    {
        Vector3 randomPoint = Random.insideUnitSphere * STRAY_RADIUS;
        randomPoint += owner.transform.position;

        NavMeshHit hitNav;
        if (NavMesh.SamplePosition(randomPoint, out hitNav, STRAY_RADIUS, NavMesh.AllAreas))
        {
            owner.NavMeshAgent.SetDestination(hitNav.position);
        }
        else
        {
            SetDestination();
        }
    }
}
