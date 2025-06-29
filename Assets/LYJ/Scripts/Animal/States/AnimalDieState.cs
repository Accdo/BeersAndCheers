using UnityEngine;

public class AnimalDieState : State_LYJ<Animal>
{
    private const float DIE_TO_DESTROY_TIME = 15f; // 시체가 유지될 시간
    private Animal owner;
    private float dieStartTime;
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
        owner.NavMeshAgent.ResetPath();
        owner.NavMeshAgent.enabled = false;
        owner.Hud.gameObject.SetActive(false);
    }

    private void DropItem()
    {
        if (owner.Data.DropItem != null)
        {
            GH_GameManager.instance.player.inventory.Add("Backpack", owner.Data.DropItem);
        }
        else
        {
            Debug.Log("아이템 없음");
        }
        // if (owner.Data.DropItems.Count == 0)
        // {
        //     Debug.Log("아이템 미존재");
        //     return;
        // }
        // GameObject dropedItem = Instantiate(owner.Data.DropItems[0], owner.transform); // 수정필요
        // dropedItem.transform.parent = null;
    }
}
