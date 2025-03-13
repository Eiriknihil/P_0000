using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Patrol-Random Wander", menuName = "Enemy Logic/Patrol Logic/Random Wander")]
public class EnemyPatrolRandomWander : EnemyPatrolSOBase
{
    [SerializeField] private float patrolRange = 10f; // Rango de patrulla
    [SerializeField] private float patrolSpeed = 2f; // Velocidad de patrulla

    private Vector3 targetPosition; // Posición objetivo actual
    private float stoppingDistance = 0.5f; // Distancia de parada

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        navAgent.speed = patrolSpeed; // Configura la velocidad de patrulla
        navAgent.stoppingDistance = stoppingDistance; // Configura la distancia de parada
        targetPosition = GetRandomPosition(); // Obtén una posición aleatoria inicial
        navAgent.SetDestination(targetPosition); // Mueve al enemigo hacia la posición objetivo
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        // Si el enemigo está en estado de aggro, cambia al estado de persecución
        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // Si el enemigo está cerca del punto objetivo, obtén un nuevo punto aleatorio
        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            targetPosition = GetRandomPosition(); // Obtén una nueva posición aleatoria
            navAgent.SetDestination(targetPosition); // Mueve al enemigo hacia la nueva posición
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange; // Genera una dirección aleatoria
        randomDirection += transform.position; // Ajusta la dirección aleatoria a la posición actual del enemigo

        // Busca un punto válido en el NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRange, NavMesh.AllAreas))
        {
            return hit.position; // Devuelve la posición válida en el NavMesh
        }

        return transform.position; // Si no se encuentra un punto válido, devuelve la posición actual
    }
}