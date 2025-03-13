using UnityEngine;

public class EnemyStrikingDistanceCheck : MonoBehaviour
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
            Debug.Log("Jugador entró en el área de ataque.");
            _enemy.SetStrikingDistanceBool(true); // Entra en AttackState
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerTarget)
        {
            Debug.Log("Jugador salió del área de ataque.");
            _enemy.SetStrikingDistanceBool(false); // Vuelve a ChaseState
        }
    }
}