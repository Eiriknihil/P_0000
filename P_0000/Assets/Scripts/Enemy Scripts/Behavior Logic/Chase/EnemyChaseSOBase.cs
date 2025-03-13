using UnityEngine;

public class EnemyChaseSOBase : ScriptableObject
{
    [Header("Chase Settings")]
    [SerializeField] private float _timeToStopChasing = 10f; // Tiempo en segundos para dejar de seguir al jugador
    [SerializeField] private float _movementSpeed = 3.5f; // Velocidad de persecución

    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected Transform playerTransform; // Referencia al transform del jugador

    // Exponemos la variable como pública para que otros scripts puedan acceder a ella
    public float TimeToStopChasing => _timeToStopChasing; // Propiedad para acceder al valor
    public float MovementSpeed => _movementSpeed; // Propiedad para acceder a la velocidad

    public virtual void Initialize(GameObject gameObject, Enemy enemy, Transform playerTransform)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;
        this.playerTransform = playerTransform; // Asigna la referencia al transform del jugador
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }

    public virtual void DoFrameUpdateLogic()
    {
        // Si el enemigo no está en estado de aggro, vuelve al estado PatrolState
        if (!enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.PatrolState);
            return;
        }

        // Si el jugador está dentro de la distancia de ataque, cambia al estado AttackState
        if (enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.ProjectileAttackState);
        }
    }

    public virtual void DoPhysicsLogic() { }
    public virtual void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
}