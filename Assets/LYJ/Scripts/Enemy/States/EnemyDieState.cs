using UnityEngine;

public class EnemyDieState : State_LYJ<Enemy>
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
        owner.Anim.SetTrigger("Die");
        owner.Anim.SetBool("Idle", false);
        owner.Anim.SetBool("Walk", false);

        DeactiveComponents();
        DropItem();
    }

    public override void Run()
    {
        base.Run();
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
        if (owner.Data.DropItems.Count == 0)
        {
            Debug.Log("아이템 미존재");
            return;
        }
        GameObject dropedItem = Instantiate(owner.Data.DropItems[0], owner.transform); // 수정필요
        dropedItem.transform.parent = null;
    }
}
