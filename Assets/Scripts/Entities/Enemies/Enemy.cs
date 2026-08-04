using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemyAI
{
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

    }


    public void Tick(Vector3 playerPosition)
    {
        sprite.flipX = playerPosition.x < transform.position.x;
        // does the navmesh move here on not setting the destination? Does it cache?
    }

    public void UpdateTarget(Vector3 target)
    {
        // update the destination only when needed
        Debug.Log($"SetDestination({target})");
        _agent.SetDestination(target);
    }


    void OnDisable()
    {
        _entityHealth.OnDeath -= DestroyEnemy;
        
    }

    void DestroyEnemy()
    {
        
        AudioManager.Instance.PlayEnemyDeath();
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }



    public void SetFrozen(bool frozen)
    {
       
        _agent.isStopped = frozen;
        Debug.Log($"{name} frozen={frozen}, isStopped={_agent.isStopped}");
    }

  
}
