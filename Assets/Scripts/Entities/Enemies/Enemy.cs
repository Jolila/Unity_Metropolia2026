using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemyAI
{
    bool waiting;
    EntityHealth _entityHealth;

    [SerializeField] AudioClip _deathSound;
    public Transform player;
    public SpriteRenderer sprite;
    UnityEngine.AI.NavMeshAgent _agent;
    Vector3 cachedTarget;
    float fakeMoveSpeed = 4f;

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

        if(GameManager.Instance.GetGameIsEnding())
        {
            _agent.isStopped = true;
            return;
        }

        if (!waiting) // suppose the navmesh agent is handling the movement?
            return;

        FakeAdvance();
    }

        void FakeAdvance()
        {

        if (!_agent.pathPending && _agent.hasPath)
        {
            _agent.Warp(transform.position);
            waiting = false;
            return;
        }

        transform.position +=
            (cachedTarget - transform.position).normalized *
            fakeMoveSpeed *
            Time.deltaTime;

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
        _agent.ResetPath();
        gameObject.SetActive(false);
    }


  
}
