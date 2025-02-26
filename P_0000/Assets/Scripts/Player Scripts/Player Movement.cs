using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Componente CharacterController para manejar movimiento físico
    [SerializeField] private CharacterController pController;
    
    // Referencia a la cámara para movimiento relativo a ella
    [SerializeField] Transform thirdPersonCamera;
    
    // Variables de configuración
    [SerializeField] private float pSpeed = 7f;
    [SerializeField] public float turnSmoothTime = 0.1f;
    
    // Variable para suavizado de rotación
    float turnSmoothVelocity;

    void Update()
    {
        // Obtener inputs raw (valores enteros: -1, 0, 1)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear vector de dirección normalizado (sin componente Y)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Solo mover si hay input significativo
        if(direction.magnitude >= 0.1f)
        {
            // Calcular ángulo de rotación respecto a la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;
            
            // Suavizar la rotación usando SmoothDampAngle
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,  // Rotación actual
                targetAngle,             // Rotación deseada
                ref turnSmoothVelocity,    // Referencia a variable de velocidad de rotación
                turnSmoothTime            // Tiempo de suavizado
            );
            
            // Aplicar la rotación al personaje
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calcular dirección de movimiento relativa a la rotación de la cámara
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Mover el controlador en la dirección calculada
            pController.Move(moveDirection.normalized * pSpeed * Time.deltaTime);
        }
    }
}