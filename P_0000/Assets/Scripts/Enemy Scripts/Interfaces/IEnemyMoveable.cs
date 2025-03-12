using UnityEngine;

public interface IEnemyMoveable
{
    Rigidbody rb { get; set; }
    void MoveEnemy(Vector3 velocity);
}