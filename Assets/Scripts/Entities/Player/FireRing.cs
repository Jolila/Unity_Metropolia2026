using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireRing : MonoBehaviour
{

    Transform _playerTransform;
    [SerializeField] float _timer = 0.5f;
    [SerializeField] float _dps = 2.0f;
    float radius = 4.9f;
    Vector2 offset = new Vector2(0f, -0.3f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _playerTransform.position;

        Vector2 center = (Vector2)transform.position + offset;
        float radiusSqr = radius * radius;

        foreach (GameObject enemy in EnemyManager.Instance.GetEnemiesForFireRing(
        transform.position))
        {
            Vector2 delta = (Vector2)enemy.transform.position - center;

            if (delta.sqrMagnitude > radiusSqr)
                continue;

            if (enemy.TryGetComponent(out EntityHealth entityHealth))
            {
                entityHealth.LoseHealth(_dps * Time.deltaTime);
            }
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
