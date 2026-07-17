using UnityEngine;

public class Enemy : MonoBehaviour
{
    EntityHealth _entityHealth;

    [SerializeField] AudioClip _deathSound;

    UnityEngine.AI.NavMeshAgent _agent;
    GameObject _target;
    // Start is called once before the first execution of Update after the Mono
    // Behaviour is created

    void OnEnable()
    {
        _entityHealth.OnDeath += DestroyEnemy;
    }


    void Awake()
    {
        _entityHealth = GetComponentInParent<EntityHealth>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
    }
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
        _entityHealth.OnDeath += DestroyEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        _agent.SetDestination(_target.transform.position);
    }

    void OnDisable()
    {
        _entityHealth.OnDeath -= DestroyEnemy;
    }

    void DestroyEnemy()
    {
        AudioManager.Instance.PlayAudio(_deathSound, AudioManager.SoundType.SFX, 1.0f, false);
        gameObject.SetActive(false);
    }
}
