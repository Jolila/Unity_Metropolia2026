using UnityEngine;
using UnityEngine.U2D;

public class NonWallCollidingEnemy : MonoBehaviour, IEnemyAI
{
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
        targetDirection = (playerPosition - transform.position).normalized;
    }

}
