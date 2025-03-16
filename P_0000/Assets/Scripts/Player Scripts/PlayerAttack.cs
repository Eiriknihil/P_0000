using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Animation Settings")]
    public string attackAnimationName = "Anim_KnightAttack"; // Nombre de la animación de ataque
    public string idleAnimationName = "Anim_KnightIdle"; // Nombre de la animación de idle
    public string runAnimationName = "Anim_KnightRun"; // Nombre de la animación de correr
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    public float attackDuration = 0.5f; // Duración manual del ataque
    public float attackStepDistance = 1f; // Distancia del pequeño paso durante el ataque

    [Header("Damage Settings")]
    public float attackDamage = 10f; // Daño del ataque

    [Header("Collider Settings")]
    public Collider attackCollider; // Collider que actuará como área de daño

    [Header("Enemy Detection Settings")]
    public float detectionRange = 5f; // Rango para detectar enemigos cercanos
    public LayerMask enemyLayer; // Capa de los enemigos
    public float rotationSpeed = 10f; // Velocidad de rotación hacia el enemigo

    private Animator pAnimator; // Referencia al Animator
    private PlayerMovement playerMovement; // Referencia al script de movimiento
    private float nextAttackTime = 0f; // Temporizador para el cooldown
    private bool isAttacking = false; // Indica si el personaje está atacando
    private Transform currentTarget; // Enemigo actual al que se está atacando

    void Start()
    {
        // Obtener la referencia al Animator y al script de movimiento
        pAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Desactivar el collider de ataque al inicio
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    void Update()
    {
        // Verificar si se puede atacar con clic izquierdo
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isAttacking)
        {
            Attack();
        }

        // Buscar el enemigo más cercano dentro del rango de detección
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        currentTarget = GetClosestEnemy(hitEnemies);

        // Rotar hacia el enemigo más cercano si hay un objetivo
        if (currentTarget != null)
        {
            RotateTowardsEnemy(currentTarget);
        }
    }

    private void Attack()
    {
        // Deshabilitar el movimiento durante el ataque
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Reproducir la animación de ataque por su nombre
        pAnimator.Play(attackAnimationName);

        // Activar el collider de ataque
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }

        // Establecer el cooldown
        nextAttackTime = Time.time + attackCooldown;

        // Indicar que el personaje está atacando
        isAttacking = true;

        // Mover al personaje un pequeño paso hacia adelante
        MoveForwardDuringAttack();

        // Desactivar el collider y finalizar el ataque después de la duración manual
        Invoke("FinishAttack", attackDuration);
    }

    private void MoveForwardDuringAttack()
    {
        // Mover al personaje un pequeño paso hacia adelante
        Vector3 step = transform.forward * attackStepDistance * Time.deltaTime;
        transform.position += step;
    }

    private void FinishAttack()
    {
        // Desactivar el collider de ataque
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        // Indicar que el ataque ha terminado
        isAttacking = false;

        // Volver a habilitar el movimiento
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Volver a la animación de idle o run
        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        // Obtener el input de movimiento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Determinar si el personaje está corriendo o quieto
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            pAnimator.Play(runAnimationName); // Reproducir animación de correr
        }
        else
        {
            pAnimator.Play(idleAnimationName); // Reproducir animación de idle
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el collider de ataque entró en contacto con un enemigo
        if (isAttacking && other.CompareTag("Enemy"))
        {
            // Obtener el componente IDamageable del enemigo
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplicar daño al enemigo
                damageable.Damage(attackDamage);
                Debug.Log($"Daño aplicado: {attackDamage}");
            }
        }
    }

    private Transform GetClosestEnemy(Collider[] enemies)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private void RotateTowardsEnemy(Transform enemy)
    {
        // Calcular la dirección hacia el enemigo
        Vector3 direction = (enemy.position - transform.position).normalized;
        direction.y = 0; // Ignorar la rotación en el eje Y

        // Rotar suavemente hacia el enemigo
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar un gizmo en el Editor para visualizar el rango de detección
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
/*using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Animation Settings")]
    public string attackAnimationName = "Anim_KnightAttack"; // Nombre de la animación de ataque
    public string idleAnimationName = "Anim_KnightIdle"; // Nombre de la animación de idle
    public string runAnimationName = "Anim_KnightRun"; // Nombre de la animación de correr
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    public float attackDuration = 0.5f; // Duración manual del ataque
    public float attackStepDistance = 1f; // Distancia del pequeño paso durante el ataque

    [Header("Damage Settings")]
    public float attackDamage = 10f; // Daño del ataque

    [Header("Collider Settings")]
    public Collider attackCollider; // Collider que actuará como área de daño

    private Animator pAnimator; // Referencia al Animator
    private PlayerMovement playerMovement; // Referencia al script de movimiento
    private float nextAttackTime = 0f; // Temporizador para el cooldown
    private bool isAttacking = false; // Indica si el personaje está atacando

    void Start()
    {
        // Obtener la referencia al Animator y al script de movimiento
        pAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Desactivar el collider de ataque al inicio
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    void Update()
    {
        // Verificar si se puede atacar con clic izquierdo
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isAttacking)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Deshabilitar el movimiento durante el ataque
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Reproducir la animación de ataque por su nombre
        pAnimator.Play(attackAnimationName);

        // Activar el collider de ataque
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }

        // Establecer el cooldown
        nextAttackTime = Time.time + attackCooldown;

        // Indicar que el personaje está atacando
        isAttacking = true;

        // Mover al personaje un pequeño paso hacia adelante
        MoveForwardDuringAttack();

        // Desactivar el collider y finalizar el ataque después de la duración manual
        Invoke("FinishAttack", attackDuration);
    }

    private void MoveForwardDuringAttack()
    {
        // Mover al personaje un pequeño paso hacia adelante
        Vector3 step = transform.forward * attackStepDistance * Time.deltaTime;
        transform.position += step;
    }

    private void FinishAttack()
    {
        // Desactivar el collider de ataque
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        // Indicar que el ataque ha terminado
        isAttacking = false;

        // Volver a habilitar el movimiento
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Volver a la animación de idle o run
        HandleMovementAnimation();
    }

    private void HandleMovementAnimation()
    {
        // Obtener el input de movimiento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Determinar si el personaje está corriendo o quieto
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            pAnimator.Play(runAnimationName); // Reproducir animación de correr
        }
        else
        {
            pAnimator.Play(idleAnimationName); // Reproducir animación de idle
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el collider de ataque entró en contacto con un enemigo
        if (isAttacking && other.CompareTag("Enemy"))
        {
            // Obtener el componente IDamageable del enemigo
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplicar daño al enemigo
                damageable.Damage(attackDamage);
                Debug.Log($"Daño aplicado: {attackDamage}");
            }
        }
    }
}
*/