using UnityEngine;

public class FollowerEnemy : MonoBehaviour, IEnemyAI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform leader;

    public float speed = 4f;
    public float lifetime;

    public bool followLeader;

    private EntityHealth health;
    public SpriteRenderer sprite;
    private Vector3 targetDirection;
    private Vector2 formationOffset;


    private void Awake()
    {
        health = GetComponent<EntityHealth>();
    }


    private void OnEnable()
    {
        health.OnDeath += Death;
        formationOffset = Random.insideUnitCircle * 2.5f;
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



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += targetDirection * speed * Time.deltaTime;
    }

    public void Tick(Vector3 playerPosition)
    {
        Transform target = leader;
        if (target == null)
        {
            target = GameManager.Instance.GetPlayerReference().transform;
        }
        Vector3 offsetTarget = target.position + (Vector3)formationOffset;
        Vector3 dir = (offsetTarget - transform.position).normalized;

        targetDirection = dir;
        sprite.flipX = dir.x < 0;
    }
}
