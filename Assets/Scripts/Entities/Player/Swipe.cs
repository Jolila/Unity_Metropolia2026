using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{

    [SerializeField] BoxCollider2D _collider;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private float _lifetime = 0.2f;

    private float dps = 5.0f; // high DPS to combat the short lifetime. A normalization for 0.2s lifetime -
                              // so torch deals 1 damage killing smallest enemies

    [SerializeField] private float swipeRadius = 1f;
    [SerializeField] private float swipeAngle = 90f;
    [SerializeField] private int arcSegments = 16;

    private Transform _player;
    private Vector2 _lookDirection;
    private float _offset;



    //public void Initialize(
    // Transform player,
    // Vector2 lookDirection,
    // float offset)
    //{
    //    _player = player;
    //    _lookDirection = lookDirection.normalized;
    //    _offset = offset;

    //    //UpdateTransform();

    //    Destroy(gameObject, _lifetime);
    //}


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_particles != null)
        {
            _particles.Play();
        }

        Destroy(gameObject, _lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateTransform();

        foreach(GameObject enemy in EnemyManager.Instance.GetEnemiesForSwipe(transform.position))
        {
            if(IsInBox(enemy.transform.position))
            {
                if (enemy.TryGetComponent(out EntityHealth entityHealth))
                {
                    entityHealth.LoseHealth(dps * Time.deltaTime);
                }
            }
        }

    }


    private bool IsInBox(Vector2 worldPosition)
    {
        Vector2 localPosition = transform.InverseTransformPoint(worldPosition);

        Vector2 center = _collider.offset;
        Vector2 halfSize = _collider.size * 0.5f;

        return
            localPosition.x >= center.x - halfSize.x &&
            localPosition.x <= center.x + halfSize.x &&
            localPosition.y >= center.y - halfSize.y &&
            localPosition.y <= center.y + halfSize.y;
    }

    private bool IsInsideSwipe(Vector2 enemyPosition)
    {
        Vector2 toEnemy =
            enemyPosition -
            (Vector2)_player.position;

        if (toEnemy.sqrMagnitude >
            swipeRadius * swipeRadius)
        {
            return false;
        }

        float angle =
            Vector2.Angle(
                _lookDirection,
                toEnemy
            );

        return angle <= swipeAngle * 0.5f;
    }

    private void UpdateTransform()
    {
        Vector3 pp = GameManager.Instance.GetPlayerReference().transform.position;
        Vector2 playerPosition = new Vector2(pp.x, pp.y);

        transform.position =
            playerPosition +
            _lookDirection * _offset;

        float angle = Mathf.Atan2(
            _lookDirection.y,
            _lookDirection.x
        ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}
