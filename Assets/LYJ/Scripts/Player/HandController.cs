using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Player_LYJ player; // temp
    [SerializeField, Tooltip("단순 무기 외형")] private List<GameObject> allWeapons;
    [SerializeField, Tooltip("손 데이터")] private List<Hand> allHands;
    [SerializeField] private Hand currentHand;
    private int currentHandIndex;
    [SerializeField] private LayerMask hitTarget;
    private bool isAttacking;
    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        currentHandIndex = 0;
    }


    void Update()
    {
        // 나중에 애니메이터 교체를 이벤트로 작성 (anim.runtime... = currenthand.anim)
        TryAttack();
        TryChangeHand();
        ControlAnim();
    }

    private void TryAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !player.DoingOtherWork)
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
            var hittable = target.GetComponent<IHittable>();
            hittable?.Damage(currentHand.Damage);
        }

        yield return new WaitForSeconds(1 / currentHand.AttackSpeed);

        isAttacking = false;
    }

    private void TryChangeHand()
    {
        if (Input.GetKeyDown(KeyCode.V) && !player.DoingOtherWork)
        {
            allWeapons[currentHandIndex].SetActive(false);
            currentHandIndex++;
            if (currentHandIndex >= allHands.Count)
            {
                currentHandIndex = 0;
            }
            currentHand = allHands[currentHandIndex];
            allWeapons[currentHandIndex].SetActive(true);
        }
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
