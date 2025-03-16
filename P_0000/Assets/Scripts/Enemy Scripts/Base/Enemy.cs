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

    [Header("Death Animation")]
    public string deathAnimationName = "Death"; // Nombre de la animación de muerte

    [SerializeField] private EnemyConfig enemyConfig; // Configuración de estados
    [SerializeField] private float timeToStopChasing = 10f; // Tiempo para dejar de perseguir al jugador

    protected Animator animator; // Referencia al Animator (ahora es protected para que las clases derivadas puedan acceder)

    private float _timeSinceLastAggro; // Temporizador para controlar el tiempo sin aggro

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

    protected virtual void Awake() // Ahora es virtual
    {
        rb = GetComponent<Rigidbody>();
        NavAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Obtener la referencia al Animator

        // Inicializa los estados con las configuraciones proporcionadas
        EnemyPatrolBaseInstance = Instantiate(enemyConfig.patrolStateConfig);
        EnemyChaseBaseInstance = Instantiate(enemyConfig.chaseStateConfig);
        EnemyProjectileAttackBaseInstance = Instantiate(enemyConfig.attackStateConfig);

        StateMachine = new EnemyStateMachine();

        // Crea los estados con las configuraciones inicializadas
        PatrolState = new EnemyPatrolState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        ProjectileAttackState = new EnemyProjectileAttackState(this, StateMachine);
    }

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;

        // Obtén la referencia al transform del jugador
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializa las instancias de los estados con la referencia al transform del jugador
        EnemyPatrolBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this, playerTransform); // Pasa el transform del jugador
        EnemyProjectileAttackBaseInstance.Initialize(gameObject, this);

        // Inicia la máquina de estados con el estado inicial
        StateMachine.Initialize(PatrolState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();

        // Si el enemigo está en estado de aggro y el jugador no está en el área de aggro, cuenta el tiempo
        if (IsAggroed && !IsPlayerInAggro())
        {
            _timeSinceLastAggro += Time.deltaTime;

            // Si el tiempo supera el límite, desactiva el aggro
            if (_timeSinceLastAggro >= timeToStopChasing)
            {
                Debug.Log("Dejando de perseguir al jugador.");
                SetAggroStatus(false); // Desactiva el aggro
                StateMachine.ChangeState(PatrolState); // Cambia a PatrolState
            }
        }
        else
        {
            _timeSinceLastAggro = 0f; // Reinicia el temporizador si el jugador está en el área de aggro
        }
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    public virtual void Damage(float damageAmount) // Ahora es virtual
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f) Die();
    }

    public virtual void Die() // Ahora es virtual
    {
        // Reproducir la animación de muerte por su nombre
        if (animator != null && !string.IsNullOrEmpty(deathAnimationName))
        {
            animator.Play(deathAnimationName); // Reproduce la animación de muerte
        }

        // Desactivar el script del enemigo
        enabled = false;

        // Deshabilitar el NavMeshAgent (si existe)
        if (NavAgent != null)
        {
            NavAgent.enabled = false;
        }

        // Destruir el objeto después de un tiempo (opcional)
        Destroy(gameObject, 5f);

        Debug.Log("El enemigo ha muerto.");
    }

    public void MoveEnemy(Vector3 destination)
    {
        NavAgent.SetDestination(destination);
    }

    public void MoveEnemy(Vector3 destination, float speed)
    {
        NavAgent.speed = speed;
        NavAgent.SetDestination(destination);
    }

    public void SetAggroStatus(bool isAggroed)
    {
        Debug.Log($"Aggro cambiado a: {isAggroed}");
        IsAggroed = isAggroed;

        if (isAggroed)
        {
            Debug.Log("Cambiando a ChaseState.");
            StateMachine.ChangeState(ChaseState);
        }
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }

    private bool IsPlayerInAggro()
    {
        return Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= 10f;
    }
}