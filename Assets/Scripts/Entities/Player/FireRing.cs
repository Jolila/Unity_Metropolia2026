using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireRing : MonoBehaviour
{

    Transform _playerTransform;
    private float _timer = 1.0f;
    private float _dps = 1f;
    float radius = 4.9f;
    Vector2 offset = new Vector2(0f, -0.3f);
    float audioEffectTimer;
    float _expendedBloodForFullLength = 60f;
    float _expendedBloodForFullDamage = 150f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {}

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

    public void SetExpended(float expended)
    {
        float t = Mathf.Lerp(1.0f, 2.0f, expended / _expendedBloodForFullLength);
        float d = Mathf.Lerp(1.0f, 3.5f, expended / _expendedBloodForFullDamage);
        audioEffectTimer = t;
        _dps = d;
        _timer = t;
    }



    public void InitializeFireAttack(Transform playerTransform)
    {
        AudioManager.Instance.PlayFireRing(audioEffectTimer);
        _playerTransform = playerTransform;
        DestroyFireRing();

    }

    void DestroyFireRing()
    {
        Destroy(gameObject, _timer);
    }
}
