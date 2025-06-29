using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { Idle, Move, Attack, Stray, Die }

public class Enemy : MonoBehaviour, IHittable
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 100f; // 수치 temp

    private EnemyHUD hud;
    public EnemyHUD Hud => hud;

    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Max(0f, value);
            hud?.ManuallyChangeHealth(currentHealth / maxHealth);
            // EventManager.Instance.TriggerEvent(EnemyEvents.HEALTH_CHANGED, new object[] { gameObject, currentHealth / maxHealth });
            if (currentHealth <= 0)
            {
                stateMachine.ChangeState(stateDic[EnemyStates.Die]);
                EventManager.Instance.TriggerEvent(EnemyEvents.DIED);
            }
        }
    }

    #region 동작 상태
    private StateMachine_LYJ<Enemy> stateMachine;
    public StateMachine_LYJ<Enemy> StateMachine => stateMachine;
    private Dictionary<EnemyStates, State_LYJ<Enemy>> stateDic;

    public bool CanAttack;
    [HideInInspector] public bool IsDead = false;
    #endregion

    private Player_LYJ chaseTarget;
    public Player_LYJ ChaseTarget => chaseTarget;
    [SerializeField] private LayerMask hitTarget;
    public LayerMask HitTarget => hitTarget;
    private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    private Animator anim;
    public Animator Anim => anim;
    private Collider coll;
    public Collider Coll => coll;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Init()
    {
        EventManager.Instance.AddListener(EnemyEvents.CHANGE_FLOOR, DestroyThis);
        hud = GetComponentInChildren<EnemyHUD>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        maxHealth = data.Health;
        CurrentHealth = data.Health;
        CanAttack = true;
        IsDead = false;

        hud.ManuallyChangeHealth(maxHealth);

        navMeshAgent.speed = data.MoveSpeed;
        SetState();
        SetStateMachine();
        stateMachine.ChangeState(stateDic[EnemyStates.Idle]);
    }

    private void SetState()
    {
        stateDic = new();
        stateDic[EnemyStates.Idle] = GetComponent<EnemyIdleState>();
        stateDic[EnemyStates.Attack] = GetComponent<EnemyAttackState>();
        stateDic[EnemyStates.Move] = GetComponent<EnemyMoveState>();
        stateDic[EnemyStates.Stray] = GetComponent<EnemyStrayState>();
        stateDic[EnemyStates.Die] = GetComponent<EnemyDieState>();
    }

    private void SetStateMachine()
    {
        stateMachine = new StateMachine_LYJ<Enemy>(this);
    }

    public void ChangeState(EnemyStates targetState)
    {
        if (IsDead) { return; }
        stateMachine.ChangeState(stateDic[targetState]);
    }

    void Update()
    {
        stateMachine?.Run();
    }

    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
    }

    public void DestroyThis(object data = null)
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EnemyEvents.CHANGE_FLOOR, DestroyThis);
    }

    public void SetTarget(Player_LYJ target)
    {
        chaseTarget = target;
    }

    public void ReleaseTarget()
    {
        chaseTarget = null;
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 centerPoint = transform.position + (transform.forward * (Data.AttackRange * 1.5f)) + (transform.up * Data.AttackHeight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint, data.AttackRange);
    }

}
