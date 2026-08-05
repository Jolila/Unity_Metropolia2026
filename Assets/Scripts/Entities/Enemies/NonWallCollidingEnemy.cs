using UnityEngine;
using UnityEngine.U2D;

public class NonWallCollidingEnemy : MonoBehaviour, IEnemyAI
{
    public Transform player;
    public Vector3 targetDirection;
    public float speed = 3.5f;
    private EntityHealth health;
    public SpriteRenderer sprite;
    public bool frozen = false;

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
        if(!GameManager.Instance.GetGameIsEnding()) transform.position += targetDirection * speed * Time.deltaTime;
    }


    private void Death()
    {
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }

    public void Tick(Vector3 playerPosition)
    {
        if (frozen) return;
        transform.position += targetDirection * speed * Time.deltaTime;
    }

    public void UpdateTarget(Vector3 playerPosition)
    {
        targetDirection = (playerPosition - transform.position).normalized;
    }

    public void SetFrozen(bool frozen)
    {
        this.frozen = frozen;
    }
}
