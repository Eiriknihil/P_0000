using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f; // Velocidad del dash
    public float dashDuration = 0.2f; // Duración del dash
    public float dashCooldown = 1f; // Tiempo de espera entre dashes
    public KeyCode dashKey = KeyCode.LeftShift; // Tecla para activar el dash

    private CharacterController pController;
    private PlayerMovement playerMovement;
    private MeshTrail meshTrail; // Referencia al script MeshTrail
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;
    private bool isDashing = false;
    private Vector3 dashDirection;

    void Start()
    {
        // Obtener referencias a los componentes necesarios
        pController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        meshTrail = GetComponent<MeshTrail>(); // Obtener el componente MeshTrail
    }

    void Update()
    {
        // Verificar si se puede activar el dash
        if (Input.GetKeyDown(dashKey) && Time.time >= nextDashTime)
        {
            StartDash();
        }

        // Manejar el dash
        if (isDashing)
        {
            if (Time.time < dashEndTime)
            {
                pController.Move(dashDirection * dashSpeed * Time.deltaTime);
            }
            else
            {
                isDashing = false; // Finalizar el dash
            }
        }
    }

    private void StartDash()
    {
        // Obtener la dirección de movimiento actual desde PlayerMovement
        Vector3 moveDirection = playerMovement.GetMovementDirection();

        if (moveDirection.magnitude > 0) // Solo dashear si hay una dirección de movimiento
        {
            isDashing = true;
            dashDirection = moveDirection.normalized; // Normalizar la dirección
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;

            // Activar el efecto del trail
            if (meshTrail != null)
            {
                meshTrail.ActivateTrail();
            }
        }
    }
}