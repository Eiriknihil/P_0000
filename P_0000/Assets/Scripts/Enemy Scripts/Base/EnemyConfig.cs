using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public EnemyPatrolSOBase patrolStateConfig;
    public EnemyChaseSOBase chaseStateConfig;
    public EnemyProjectileAttackSOBase attackStateConfig;
    
}