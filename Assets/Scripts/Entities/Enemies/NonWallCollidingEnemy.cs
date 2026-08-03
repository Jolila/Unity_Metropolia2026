using UnityEngine;
using UnityEngine.U2D;

public class NonWallCollidingEnemy : MonoBehaviour, IEnemyAI
{
    public Transform player;
    public Vector3 targetDirection;
    public float speed = 3.5f;
    private EntityHealth health;
    public SpriteRenderer sprite;
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        health = GetComponent<EntityHealth>();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        health.OnDeath += Death;
    }

    private void OnDisable()
    {
        health.OnDeath -= Death;
    }

    private void Update()
    {
        transform.position += targetDirection * speed * Time.deltaTime;
    }


    private void Death()
    {
        AudioManager.Instance.PlayAudio(deathSound, AudioManager.SoundType.SFX, 1f, false);
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }

    public void Tick(Vector3 playerPosition)
    {
        
        
    }

    public void UpdateTarget(Vector3 playerPosition)
    {
        targetDirection = (playerPosition - transform.position).normalized;
    }
}
