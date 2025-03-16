using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake(); // Llama al Awake de la clase base
    }

    public override void Die()
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

        // Deshabilitar el Rigidbody (si existe)
        if (rb != null)
        {
            rb.isKinematic = true; // Hacer que el Rigidbody no reaccione a las físicas
        }

        // Destruir el objeto después de un tiempo (opcional)
        Destroy(gameObject, 2f);

        Debug.Log("El esqueleto ha muerto.");
    }
}