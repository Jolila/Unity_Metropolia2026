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

        switch (Time.frameCount % 6)
        {
            case 0:
                SpawnRatSquad();
                break;
            case 1:
                SpawnBats();
                break;
            case 2:
                SpawnZombiSquad();
                break;
            case 3:
                SpawnSlimeSquad();
                break;


        }


    }


    void SpawnRatSquad()
    {

        Vector3 leaderPos = _locationsManager.GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.RatLeader,
                leaderPos,
                Quaternion.identity);

        if (leader == null)
            return;
        EnemyManager.Instance.RegisterEnemy(leader);
        Enemy leaderEnemy = leader.GetComponent<Enemy>();

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    EnemyType.RatFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);

            if (follower == null)
                continue;
            EnemyManager.Instance.RegisterEnemy(follower);
            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;
            
        }

    }

    void SpawnBats()
    {

        int amount = Random.Range(0, 7);
        for(int i = 0; i < amount; ++i)
        {
            GameObject bat  = EnemyPoolManager.Instance.Get(EnemyType.Bat,
                _locationsManager.GetRandomInnerWallSpawn(),
                Quaternion.identity,
                GameManager.Instance.GetPlayerReference().transform.position);
            if(bat != null) EnemyManager.Instance.RegisterEnemy(bat);
        }
        
    }


    void SpawnSlimeSquad()
    {
        Vector3 leaderPos = _locationsManager.GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.SlimeLeader,
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
                    EnemyType.SlimeFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);

            if (follower == null)
                continue;

            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;

        }
    }

    void SpawnZombiSquad()
    {
        Vector3 leaderPos = _locationsManager.GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.ZombieLeader,
                leaderPos,
                Quaternion.identity);

       

        if (leader == null)
            return;
        EnemyManager.Instance.RegisterEnemy(leader);

        Enemy leaderEnemy = leader.GetComponent<Enemy>();

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    EnemyType.ZombieFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);
            

            if (follower == null)
                continue;
            EnemyManager.Instance.RegisterEnemy(follower);

            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;

        }
    }
}
