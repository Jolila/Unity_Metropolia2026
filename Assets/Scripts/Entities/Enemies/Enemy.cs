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

    void Start()
    {
        _entityHealth.OnDeath += DestroyEnemy;
    }

    public void Tick(Vector3 playerPosition)
    {
        sprite.flipX = playerPosition.x < transform.position.x;
        // does the navmesh move here on not setting the destination? Does it cache?
    }

    public void UpdateTarget(Vector3 target)
    {
        // update the destination only when needed
        _agent.SetDestination(target);
    }


    void OnDisable()
    {
        _entityHealth.OnDeath -= DestroyEnemy;
        GameManager.Instance.RegisterKill();
    }

    void DestroyEnemy()
    {
        AudioManager.Instance.PlayAudio(_deathSound, AudioManager.SoundType.SFX, 1.0f, false);
        gameObject.SetActive(false);
    }
}
