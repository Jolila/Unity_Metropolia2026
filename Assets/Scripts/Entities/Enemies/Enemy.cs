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

    // Start is called once before the first execution of Update after the Mono
    // Behaviour is created

    void OnEnable()
    {
        _entityHealth.OnDeath += DestroyEnemy;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        
    }


    void Awake()
    {
        _entityHealth = GetComponentInParent<EntityHealth>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
    }

    void Update()
    {
        if (!waiting)
            return;

        Debug.Log(
            $"Pending:{_agent.pathPending} " +
            $"HasPath:{_agent.hasPath} " +
            $"Status:{_agent.pathStatus} " +
            $"OnMesh:{_agent.isOnNavMesh} " +
            $"Vel:{_agent.velocity.magnitude:F2}");

        if (_agent.hasPath)
            waiting = false;
    }


    public void Tick(Vector3 playerPosition)
    {
        sprite.flipX = playerPosition.x < transform.position.x;
        // does the navmesh move here on not setting the destination? Does it cache?
    }

    public void UpdateTarget(Vector3 target)
    {
       

     
          

        bool ok = _agent.SetDestination(target);
        Debug.Log(ok);

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



    public void SetFrozen(bool frozen)
    {
       
        _agent.isStopped = frozen;
        Debug.Log($"{name} frozen={frozen}, isStopped={_agent.isStopped}");
    }

  
}
