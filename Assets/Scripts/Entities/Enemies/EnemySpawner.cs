using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] EnemySpawnLocationsManager _locationsManager;
    void Start()
    {
        
    }


    public void SpawnCountDownEnemies()
    {

            SpawnRatSquad();
            SpawnBats();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetState() != GameState.Playing) return;

        // do some interesting logic here



        SpawnRatSquad();
        SpawnBats();
    }


    void SpawnRatSquad()
    {

        Vector3 leaderPos = _locationsManager.GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                PoolID.RatLeader,
                leaderPos,
                Quaternion.identity);

        if (leader == null)
            return;

        Enemy leaderEnemy = leader.GetComponent<Enemy>();

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    PoolID.RatFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);

            if (follower == null)
                continue;

            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;
            
        }

    }

    void SpawnBats()
    {

        int amount = Random.Range(0, 7);
        for(int i = 0; i < amount; ++i)
        {
            EnemyPoolManager.Instance.Get(PoolID.Bat, _locationsManager.GetRandomInnerWallSpawn(), Quaternion.identity, GameManager.Instance.GetPlayerReference().transform.position);
        }
        
    }
}
