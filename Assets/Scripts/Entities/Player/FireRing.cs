using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireRing : MonoBehaviour
{

    Transform _playerTransform;
    [SerializeField] float _timer = 2.0f;
    [SerializeField] float _dps = 10.0f;
    float radius = 3.7f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _playerTransform.position;

        foreach (GameObject enemy in EnemyManager.Instance.GetEnemiesForFireRing(
           transform.position))
        {

            Vector2 delta = enemy.transform.position - transform.position;

            if (delta.sqrMagnitude > radius)
                continue;


            if (enemy.TryGetComponent(out EntityHealth entityHealth))
            {
                entityHealth.LoseHealth(_dps);
               
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
