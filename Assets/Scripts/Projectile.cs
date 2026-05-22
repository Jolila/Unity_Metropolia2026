using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float _travelSpeed;
    [SerializeField] float _damage;
    [SerializeField] Rigidbody2D _rb;

    public void InitializeProjectile(Vector2 direction)
    {
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
            Debug.Log("Projectile hits wall");
            DestroyProjectile();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
