using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public NavMeshAgent NavAgent { get; set; }
    public Rigidbody rb { get; set; }
    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }

    [SerializeField] private EnemyPatrolSOBase EnemyPatrolBase;
    [SerializeField] private EnemyChaseSOBase EnemyChaseBase;
    [SerializeField] private EnemyProjectileAttackSOBase EnemyProjectileAttackBase;

    public EnemyPatrolSOBase EnemyPatrolBaseInstance { get; set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    public EnemyProjectileAttackSOBase EnemyProjectileAttackBaseInstance { get; set; }

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyProjectileAttackState ProjectileAttackState { get; set; }

    public enum AnimationTriggerType
    {
        Walk,
        Attack
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        NavAgent = GetComponent<NavMeshAgent>();

        EnemyPatrolBaseInstance = Instantiate(EnemyPatrolBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
        EnemyProjectileAttackBaseInstance = Instantiate(EnemyProjectileAttackBase);

        StateMachine = new EnemyStateMachine();

        PatrolState = new EnemyPatrolState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        ProjectileAttackState = new EnemyProjectileAttackState(this, StateMachine);
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;

        EnemyPatrolBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyProjectileAttackBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(PatrolState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f) Die();
    }

    public void Die()
    {
        Destroy(gameObject, 2f);
        enabled = false;
    }

    public void MoveEnemy(Vector3 velocity)
    {
        NavAgent.SetDestination(velocity);
    }

    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }
}