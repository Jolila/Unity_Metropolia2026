using UnityEngine;
using UnityEngine.U2D;

public class NonWallCollidingEnemy : MonoBehaviour, IEnemyAI
{
    public Vector3 targetDirection;
    public Vector3 personalTargetOffset;
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
        personalTargetOffset = Random.insideUnitCircle * 0.15f;
    }

    private void OnDisable()
    {
        health.OnDeath -= Death;
    }

    private void Update()
    {
        if(GameManager.Instance.GetState() != GameState.Ending) transform.position += targetDirection * speed * Time.deltaTime;
    }


    private void Death()
    {
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }

    public void Tick(Vector3 playerPosition)
    {
        targetDirection = (playerPosition + personalTargetOffset - transform.position).normalized;
    }

}
