using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct Chase", menuName = "Enemy Logic/Chase Logic/Direct Chase")]
public class EnemyChaseDirectToPlayer : EnemyChaseSOBase
{
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        // Si el enemigo está aggro, sigue al jugador
        if (enemy.IsAggroed)
        {
            Vector3 destination = playerTransform.position; // Usa la posición del jugador como destino
            enemy.MoveEnemy(destination, MovementSpeed); // Pasa la velocidad del SO
        }
    }
}