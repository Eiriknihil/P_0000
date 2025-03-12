using UnityEngine;

[CreateAssetMenu(fileName = "Patrol-Random Wander", menuName = "Enemy Logic/Patrol Logic/Random Wander")]
public class EnemyPatrolRandomWander : EnemyPatrolSOBase
{
    [SerializeField] private float patrolRange = 10f;
    [SerializeField] private float patrolSpeed = 2f;

    private Vector3 targetPosition;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        navAgent.speed = patrolSpeed;
        targetPosition = GetRandomPosition();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            targetPosition = GetRandomPosition();
        }

        navAgent.SetDestination(targetPosition);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, patrolRange, UnityEngine.AI.NavMesh.AllAreas);
        return hit.position;
    }
}