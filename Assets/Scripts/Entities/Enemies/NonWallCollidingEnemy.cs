using UnityEngine;
using UnityEngine.U2D;

public class NonWallCollidingEnemy : MonoBehaviour, IEnemyAI
{
    public Transform player;
    public Vector3 targetDirection;
    public float speed = 3.5f;
    private EntityHealth health;
    public SpriteRenderer sprite;

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


    private void Death()
    {
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }

    public void Tick(Vector3 playerPosition)
    {
        transform.position += targetDirection * speed * Time.deltaTime;
    }

    public void UpdateTarget(Vector3 playerPosition)
    {
        targetDirection = (playerPosition - transform.position).normalized;
    }
}
