using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float airTime = 0.7f;    // Tiempo en alcanzar la altura máxima
    [SerializeField] private float gravity = -9.81f;  // Gravedad base
    [SerializeField] private float fallMultiplier = 8f; // Multiplicador de caída

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;    // Punto de detección de suelo
    [SerializeField] private float groundDistance = 0.4f; // Radio de detección
    [SerializeField] private LayerMask groundLayer;   // Capa(s) del suelo

    private CharacterController pController;
    private Vector3 velocity;
    private bool isGrounded;
    private float initialJumpVelocity; // Velocidad inicial calculada automáticamente

    void Start()
    {
        pController = GetComponent<CharacterController>();
        CalculateJumpPhysics(); // Calcular velocidad inicial basada en airTime
    }

    void Update()
    {
        // Detectar si está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Resetear velocidad vertical al tocar suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Lógica de salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = initialJumpVelocity; // Usar velocidad calculada
        }

        // Aplicar gravedad variable
        if (velocity.y < 0) // Si está cayendo
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else // Si está subiendo
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Mover el controlador
        pController.Move(velocity * Time.deltaTime);
    }

    // Calcular la velocidad inicial basada en airTime (fórmula física)
    private void CalculateJumpPhysics()
    {
        float timeToPeak = airTime;
        initialJumpVelocity = (Mathf.Abs(gravity) * timeToPeak); // v = g * t
    }

    // Actualizar cálculos si se modifican variables en el Inspector
    private void OnValidate()
    {
        CalculateJumpPhysics();
    }
}