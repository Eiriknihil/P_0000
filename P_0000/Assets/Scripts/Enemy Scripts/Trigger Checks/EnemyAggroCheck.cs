using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
    public GameObject PlayerTarget { get; set; }
    private Enemy _enemy;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerTarget)
        {
            Debug.Log("Jugador entró en el área de aggro.");
            _enemy.SetAggroStatus(true); // Activa el aggro
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerTarget)
        {
            Debug.Log("Jugador salió del área de aggro.");
            // No desactives el aggro aquí, solo notifica que el jugador salió
        }
    }
}