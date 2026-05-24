using UnityEngine;

public class FireRing : MonoBehaviour
{

    Transform _playerTransform;
    [SerializeField] float _timer = 2.0f;
    [SerializeField] float _dps = 10.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _playerTransform.position;

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.LoseHealth(Time.fixedDeltaTime * _dps);
            
        }
    }

    public void InitializeFireAttack(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        DestroyFireRing();
    }

    void DestroyFireRing()
    {
        Destroy(gameObject, _timer);
    }
}
