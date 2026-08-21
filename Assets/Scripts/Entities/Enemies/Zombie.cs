using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IEnemyAI
{

    bool waiting;
    EntityHealth _entityHealth;

    [SerializeField] AudioClip _deathSound;
    public SpriteRenderer sprite;
    UnityEngine.AI.NavMeshAgent _agent;
    Vector3 cachedTarget;
    BloodDropletTier maxDropTier = BloodDropletTier.Large;

    void OnEnable()
    {
        _entityHealth.OnDeath += DestroyEnemy;
        waiting = true;
    }


    void Awake()
    {
        _entityHealth = GetComponentInParent<EntityHealth>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    void Update()
    {

        if (GameManager.Instance.GetState() == GameState.Ending)
        {
            _agent.isStopped = true;
            return;
        }

        if (!waiting) // suppose the navmesh agent is handling the movement?
            return;

    }


    public void Tick(Vector3 playerPosition)
    {
        cachedTarget = playerPosition;
        sprite.flipX = playerPosition.x < transform.position.x;
        _agent.SetDestination(playerPosition);
    }




    void OnDisable()
    {
        _entityHealth.OnDeath -= DestroyEnemy;

    }

    void DestroyEnemy()
    {

        AudioManager.Instance.PlayEnemyDeath();
        GameManager.Instance.RegisterKill();
        EnemyManager.Instance.UnregisterEnemy(gameObject);
        _agent.ResetPath();
        gameObject.SetActive(false);
        EnemyManager.Instance.GetBloodSystem().TrySpawnDroplet(maxDropTier, transform.position);
    }


}
