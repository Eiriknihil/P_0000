using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private CharacterController pController;
    [SerializeField] private Animator pAnimator;
    [SerializeField] private Transform thirdPersonCamera;
    [SerializeField] private Transform groundCheck;

    [Header("Movement Settings")]
    [SerializeField] private float pSpeed = 7f; // Velocidad de movimiento del personaje
    [SerializeField] private float turnSmoothTime = 0.1f; // Tiempo de suavizado de rotación
    private float turnSmoothVelocity; // Variable de referencia para el suavizado de rotación

    [Header("Jump Settings")]
    [SerializeField] private float airTime = 0.7f; // Tiempo que tarda en alcanzar la altura máxima del salto
    [SerializeField] private float gravity = -9.81f; // Fuerza de la gravedad
    [SerializeField] private float fallMultiplier = 8f; // Multiplicador de gravedad en caída
    [SerializeField] private float groundDistance = 0.4f; // Radio de detección de suelo
    [SerializeField] private LayerMask groundLayer; // Capas consideradas como suelo
    [SerializeField] private float coyoteTime = 0.15f; // Tiempo en segundos para realizar el salto después de dejar una plataforma

    [Header("Animations")]
    [SerializeField] private string runAnimation = "Anim_KnightRun";
    [SerializeField] private string idleAnimation = "Anim_KnightIdle";
    [SerializeField] private string jumpAnimation = "Anim_KnightJump";

    // Hashes para optimizar las transiciones de animación
    private int _runHash;
    private int _idleHash;
    private int _jumpHash;
    private int currentAnimationHash;
    
    private Vector3 velocity; // Velocidad actual del personaje
    private float initialJumpVelocity; // Velocidad inicial calculada del salto
    private bool isGrounded; // Si el personaje está en el suelo
    private float coyoteTimeCounter; // Temporizador para el coyote time

    void Start()
    {
        // Inicializar componentes y cálculos físicos
        pController = GetComponent<CharacterController>();
        pAnimator = GetComponent<Animator>();
        CalculateJumpPhysics();
        
        // Convertir nombres de animaciones a hashes numéricos para mejor rendimiento
        _runHash = Animator.StringToHash(runAnimation);
        _idleHash = Animator.StringToHash(idleAnimation);
        _jumpHash = Animator.StringToHash(jumpAnimation);
        currentAnimationHash = _idleHash;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        CheckAnimation();
        HandleCoyoteTime(); // Actualizar el coyote time cada frame
    }

    private void HandleMovement()
    {
        // Obtener inputs de movimiento y normalizar la dirección
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            // Calcular rotación suavizada hacia la dirección de movimiento relativa a la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );
            
            // Aplicar rotación y movimiento
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            pController.Move(moveDirection.normalized * pSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        // Detección de suelo usando una esfera en el punto groundCheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Resetear velocidad vertical y coyote time al tocar suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            coyoteTimeCounter = coyoteTime; // Resetear coyote time
        }

        // Lógica de salto con coyote time
        if (Input.GetButtonDown("Jump") && (isGrounded || coyoteTimeCounter > 0))
        {
            velocity.y = initialJumpVelocity;
            SafeCrossFade(_jumpHash, 0.1f);
            coyoteTimeCounter = 0; // Consumir el coyote time al saltar
        }

        ApplyGravity();
        pController.Move(velocity * Time.deltaTime);
    }

    private void HandleCoyoteTime()
    {
        // Reducir el coyote time solo cuando no está en el suelo
        if (!isGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ApplyGravity()
    {
        // Aplicar gravedad con multiplicador en caída
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void CheckAnimation()
    {   
        // Manejar transiciones de animación
        if (!isGrounded)
        {
            SafeCrossFade(_jumpHash, 0.1f);
        }
        else
        {
            // Obtener inputs para animaciones terrestres
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                SafeCrossFade(_runHash, 0.2f);
            }
            else
            {
                SafeCrossFade(_idleHash, 0.2f);
            }
        }
    }

    private void SafeCrossFade(int hash, float duration)
    {
        // Transición segura entre animaciones
        if (pAnimator.HasState(0, hash) && currentAnimationHash != hash)
        {
            pAnimator.CrossFade(hash, duration);
            currentAnimationHash = hash;
        }
    }

    private void CalculateJumpPhysics()
    {
        // Calcular velocidad inicial usando fórmula física: v = √(2gh)
        initialJumpVelocity = Mathf.Abs(gravity) * airTime;
    }

    private void OnValidate()
    {
        // Recalcular automáticamente al modificar valores en el Inspector
        CalculateJumpPhysics();
    }

    // Visualización del groundCheck en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}