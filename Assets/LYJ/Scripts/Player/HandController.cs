using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Player_LYJ player; // temp
    [SerializeField] private Hand currentHand;
    [SerializeField] private LayerMask hitTarget;
    private bool isAttacking;
    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        // 나중에 애니메이터 교체를 이벤트로 작성 (anim.runtime... = currenthand.anim)
        TryAttack();
        ControlAnim();
    }

    private void TryAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");
        Vector3 centerPoint = transform.position + (transform.forward * currentHand.Distance);

        Collider[] hitTargets = Physics.OverlapSphere(centerPoint, currentHand.Radius, hitTarget);

        foreach (var target in hitTargets)
        {
            var enemy = target.GetComponent<Enemy>();
            enemy?.Damage(currentHand.Damage);
        }

        yield return new WaitForSeconds(1 / currentHand.AttackSpeed);

        isAttacking = false;
    }

    private void ControlAnim()
    {
        anim.SetBool("Walk", player.IsWalk);
        anim.SetBool("Run", player.IsRun);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centerPoint = transform.position + (transform.forward * currentHand.Distance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint, currentHand.Radius);
    }

}
