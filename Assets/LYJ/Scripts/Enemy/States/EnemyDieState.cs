using UnityEngine;

public class EnemyDieState : State_LYJ<Enemy>
{
    private const float DIE_TO_DESTROY_TIME = 15f; // 시체가 유지될 시간
    private Enemy owner;
    private float dieStartTime;

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
        owner.Anim.SetTrigger("Die");
        owner.Anim.SetBool("Idle", false);
        owner.Anim.SetBool("Walk", false);
        dieStartTime = Time.time;

        DeactiveComponents();
        DropItem();
    }

    public override void Run()
    {
        base.Run();
        if (Time.time - dieStartTime >= DIE_TO_DESTROY_TIME)
        {
            owner.DestroyThis();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void DeactiveComponents()
    {
        owner.IsDead = true;
        owner.Coll.enabled = false;
        owner.ReleaseTarget();
        owner.NavMeshAgent.ResetPath();
        owner.NavMeshAgent.enabled = false;
        owner.Hud.gameObject.SetActive(false);
    }

    private void DropItem()
    {
        GameObject dropedItem = Instantiate(owner.Data.DropItems[0], owner.transform); // 수정필요
        dropedItem.transform.parent = null;
    }
}
