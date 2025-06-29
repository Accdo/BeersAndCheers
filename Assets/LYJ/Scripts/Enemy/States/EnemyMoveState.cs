using System.Collections;
using UnityEngine;

public class EnemyMoveState : State_LYJ<Enemy>
{
    private Enemy owner;
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
        // playingAudio = false;
    }

    public override void Run()
    {
        base.Run();
        if (owner.ChaseTarget != null && Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) <= owner.Data.AttackRange * 1.5f)
        {
            owner.ChangeState(EnemyStates.Attack);
            return;
        }
        if (owner.ChaseTarget != null && Vector3.Distance(owner.transform.position, owner.ChaseTarget.transform.position) > owner.Data.AttackRange * 1.5f)
        {
            owner.NavMeshAgent.SetDestination(owner.ChaseTarget.transform.position);
        }
        if (!owner.ChaseTarget)
        {
            owner.ChangeState(EnemyStates.Stray);
            return;
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

    IEnumerator PlayWalkAudio()
    {
        playingAudio = true;
        Debug.Log("추적 오디오 실행");
        owner.PlayAudio(EnemyStates.Move);
        yield return new WaitForSeconds(0.8f);
        playingAudio = false;
    }
}
