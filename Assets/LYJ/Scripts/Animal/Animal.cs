using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalStates { Idle, Stray, Die }

public class Animal : MonoBehaviour, IHittable
{
    [SerializeField] private AnimalData data;
    public AnimalData Data => data;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 50f; // 수치 temp

    private EnemyHUD hud;
    public EnemyHUD Hud => hud;

    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Max(0f, value);
            hud?.ManuallyChangeHealth(currentHealth / maxHealth);
            if (currentHealth <= 0)
            {
                stateMachine.ChangeState(stateDic[AnimalStates.Die]);
                EventManager.Instance.TriggerEvent(AnimalEvents.DIED);
            }
        }
    }

    #region 동작 상태
    private StateMachine_LYJ<Animal> stateMachine;
    public StateMachine_LYJ<Animal> StateMachine => stateMachine;
    private Dictionary<AnimalStates, State_LYJ<Animal>> stateDic;
    [HideInInspector] public bool IsDead = false;
    #endregion

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

    public void Start() // 나중에 init으로 변경
    {
        hud = GetComponentInChildren<EnemyHUD>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        maxHealth = data.Health;
        CurrentHealth = data.Health;
        IsDead = false;

        hud.ManuallyChangeHealth(maxHealth);

        navMeshAgent.speed = data.MoveSpeed;
        SetState();
        SetStateMachine();
        stateMachine.ChangeState(stateDic[AnimalStates.Idle]);
    }

    private void SetState()
    {
        stateDic = new();
        stateDic[AnimalStates.Idle] = GetComponent<AnimalIdleState>();
        stateDic[AnimalStates.Stray] = GetComponent<AnimalStrayState>();
        stateDic[AnimalStates.Die] = GetComponent<AnimalDieState>();
    }

    private void SetStateMachine()
    {
        stateMachine = new StateMachine_LYJ<Animal>(this);
    }

    public void ChangeState(AnimalStates targetState)
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

    public void DestroyThis()
    {
        Destroy(this);
    }
}
