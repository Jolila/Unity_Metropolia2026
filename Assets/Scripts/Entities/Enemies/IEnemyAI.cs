using UnityEngine;

public interface IEnemyAI
{
    void Tick(Vector3 playerPosition);
    void UpdateTarget(Vector3 target);
    void SetFrozen(bool frozen);
}
