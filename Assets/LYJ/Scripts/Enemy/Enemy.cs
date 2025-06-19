using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { Idle, Move, Attack, Hit, Die }

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    private float currentHealth;
    private float maxHealth = 100f; // 수치 temp
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Max(0f, value);
            EventManager.Instance.TriggerEvent("EnemyHealthChanged", currentHealth / maxHealth);
            if (currentHealth <= 0)
            {
                EventManager.Instance.TriggerEvent("EnemyDied", data.EnemyName);
            }
        }
    }

    private Vector3 firstPos = new Vector3(0, 1, 10); // 수치는 temp
    public Vector3 FirstPos => firstPos;

    #region 동작 상태
    private StateMachine_LYJ<Enemy> stateMachine;
    public StateMachine_LYJ<Enemy> StateMachine => stateMachine;
    private Dictionary<EnemyStates, State_LYJ<Enemy>> stateDic;
    
    public bool CanAttack = true;// 수치 temp
    #endregion

    private Player_LYJ target;
    public Player_LYJ Target => target;
    private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    private Animator anim;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // anim = GetComponent<Animator>();

        SetState();
        SetStateMachine();
    }

    private void SetState()
    {
        stateDic = new();
        stateDic[EnemyStates.Idle] = GetComponent<EnemyIdleState>();
        stateDic[EnemyStates.Attack] = GetComponent<EnemyAttackState>();
        stateDic[EnemyStates.Move] = GetComponent<EnemyMoveState>();
    }

    private void SetStateMachine()
    {
        stateMachine = new StateMachine_LYJ<Enemy>(this);
        stateMachine.ChangeState(stateDic[EnemyStates.Idle]);
    }

    public void ChangeState(EnemyStates targetState)
    {
        stateMachine.ChangeState(stateDic[targetState]);
    }

    public void Init(EnemyData data, Vector3 InitPos)
    {
        this.data = data;
        maxHealth = data.Health;
        CurrentHealth = data.Health;
        navMeshAgent.speed = data.MoveSpeed;
        firstPos = InitPos;
        CanAttack = true;
        stateMachine.ChangeState(stateDic[EnemyStates.Idle]);
        // 애니메이터 교체 (anim.runtime... = data.anim)
    }

    void Update()
    {
        stateMachine.Run();
    }



    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
    }

    public void SetTarget(Player_LYJ target)
    {
        this.target = target;
    }

    public void ReleaseTarget()
    {
        target = null;
    }
}
