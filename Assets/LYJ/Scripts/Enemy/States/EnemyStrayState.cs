using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStrayState : State_LYJ<Enemy>
{
    private const float STRAY_RADIUS = 15f;
    private Enemy owner;
    private float strayStartTime;
    private float strayingTime;
    private bool playingAudio;
    public override void Enter(Enemy owner)
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
        strayingTime = Random.Range(4f, 6f);
        strayStartTime = Time.time;
        SetDestination();
        // playingAudio = false;
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
        if (Time.time - strayStartTime >= strayingTime)
        {
            owner.ChangeState(EnemyStates.Idle);
            return;
        }
        if (!owner.NavMeshAgent.pathPending && owner.NavMeshAgent.remainingDistance < 0.5f)
        {
            SetDestination();
        }
        if (!playingAudio)
        {
            StartCoroutine(PlayWalkAudio());
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
    IEnumerator PlayWalkAudio()
    {
        playingAudio = true;
        Debug.Log("추적 오디오 실행");
        owner.PlayAudio(EnemyStates.Stray);
        yield return new WaitForSeconds(0.8f);
        playingAudio = false;
    }
}
