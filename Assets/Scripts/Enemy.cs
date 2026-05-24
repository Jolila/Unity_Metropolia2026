using UnityEngine;

public class Enemy : MonoBehaviour
{
    EntityHealth _entityHealth;

    [SerializeField] AudioClip _deathSound;
    // Start is called once before the first execution of Update after the Mono
    // Behaviour is created

    void Awake()
    {
        _entityHealth = GetComponent<EntityHealth>();
    }
    void Start()
    {
        _entityHealth.OnDeath += DestroyEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        _entityHealth.OnDeath -= DestroyEnemy;
    }

    void DestroyEnemy()
    {
        AudioManager.Instance.PlayAudio(_deathSound, AudioManager.SoundType.SFX, 1.0f, false);
        Destroy(gameObject);
    }
}
