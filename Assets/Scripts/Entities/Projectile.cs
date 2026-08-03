using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float _travelSpeed;
    [SerializeField] float _damage;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] AudioClip _enemyHitSound;

    public void InitializeProjectile(Vector2 direction)
    {
        gameObject.SetActive(true);
        Debug.Log("Shootin projectile");
        Launch(direction);
    }

    void Launch(Vector2 direction)
    {
        Vector2 movement = direction.normalized * _travelSpeed;
        _rb.linearVelocity = movement;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            DestroyProjectileOnTerrain();
        }
        if(collision.gameObject.CompareTag("Enemy"))
        {
            DealDamage(collision.gameObject);
            DestroyProjectileOnEnemy();
        }
    }

    void DealDamage(GameObject target)
    {
        if(target.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.LoseHealth(_damage);
            AudioManager.Instance.PlayAudio(_enemyHitSound, AudioManager.SoundType.SFX, 1.0f, false);
        }
    }


    void DestroyProjectile()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    void DestroyProjectileOnTerrain()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        GameManager.Instance.RegisterMiss();
    }

    void DestroyProjectileOnEnemy()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        GameManager.Instance.RegisterHit();
    }


}
