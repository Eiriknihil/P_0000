using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Projectile", menuName = "Enemy Logic/Attack Logic/Single Projectile")]
public class EnemyAttackSingleProjectile : EnemyProjectileAttackSOBase
{
    [Header("Projectile Settings")]
    [SerializeField] private Rigidbody BulletPrefab;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _timeBetweenShots = 2f;
    
    [Header("Exit Conditions")]
    [SerializeField] private float _timeTillExit = 3f;
    [SerializeField] private float _distanceToCountExit = 3f;

    [Header("Spawn Point Settings")]
    [Tooltip("Nombre del GameObject hijo que actuará como punto de spawn")]
    [SerializeField] private string _spawnPointName = "ProjectileSpawn"; // Nombre del Empty

    private Transform _projectileSpawnPoint;
    private float _timer;
    private float _exitTimer;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        
        // Busca el punto de spawn entre los hijos del enemigo
        _projectileSpawnPoint = enemy.transform.Find(_spawnPointName);
        
        // Mensaje de advertencia si no se encuentra (opcional)
        if (_projectileSpawnPoint == null)
        {
            Debug.LogWarning($"No se encontró el punto de spawn '{_spawnPointName}'. Se usará la posición del enemigo.", enemy.gameObject);
        }
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        enemy.MoveEnemy(Vector3.zero);

        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;

            // Dirección hacia el jugador
            Vector3 dir = (playerTransform.position - enemy.transform.position).normalized;

            // Posición de spawn (usa el punto asignado o la posición del enemigo)
            Vector3 spawnPosition = _projectileSpawnPoint != null ? 
                _projectileSpawnPoint.position : 
                enemy.transform.position;

            // Instancia el proyectil
            Rigidbody bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
            bullet.linearVelocity = dir * _bulletSpeed;
        }

        // Lógica para cambiar de estado si el jugador se aleja
        if (Vector3.Distance(playerTransform.position, enemy.transform.position) > _distanceToCountExit)
        {
            _exitTimer += Time.deltaTime;
            if (_exitTimer > _timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        }
        else
        {
            _exitTimer = 0f;
        }

        _timer += Time.deltaTime;
    }

    // (El resto de métodos permanecen igual)
    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) => base.DoAnimationTriggerEventLogic(triggerType);
    public override void DoEnterLogic() => base.DoEnterLogic();
    public override void DoExitLogic() => base.DoExitLogic();
    public override void DoPhysicsLogic() => base.DoPhysicsLogic();
    public override void ResetValues() => base.ResetValues();
}