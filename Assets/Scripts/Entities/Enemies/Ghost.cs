using UnityEngine;

public class Ghost : MonoBehaviour, IEnemyAI
{
    private EntityHealth health;
    public Vector3 targetDirection;
    public float speed = 3.5f;
    public SpriteRenderer sprite;
    BloodDropletTier maxDropTier = BloodDropletTier.Large;

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
        if (GameManager.Instance.GetState() != GameState.Ending) transform.position += targetDirection * speed * Time.deltaTime;
    }


    private void Death()
    {
        GameManager.Instance.RegisterKill();
        EnemyManager.Instance.GetBloodSystem().TrySpawnDroplet(maxDropTier, transform.position);
        EnemyManager.Instance.UnregisterEnemy(gameObject);
        PlaySound();
        gameObject.SetActive(false);
    }

    public void Tick(Vector3 playerPosition)
    {
        targetDirection = playerPosition.normalized;
    }

   

    private void PlaySound()
    {
        AudioManager.Instance.PlayGhostDeath();
    }

}
