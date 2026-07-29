using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Transform player;
    public float speed = 3.5f;
    private EntityHealth health;
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        health = GetComponent<EntityHealth>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }


    private void Death()
    {
        Debug.Log("Ghost is killed");
        AudioManager.Instance.PlayAudio(deathSound, AudioManager.SoundType.SFX, 1f, false);
        GameManager.Instance.RegisterKill();
        gameObject.SetActive(false);
    }
}
