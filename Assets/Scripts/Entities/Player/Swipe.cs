using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{

    [SerializeField] BoxCollider2D _collider;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private float _lifetime = 0.2f;
    private Camera mainCamera;

    private float dps = 5.0f; // high DPS to combat the short lifetime. A normalization for 0.2s lifetime -
                              // so torch deals 1 damage killing smallest enemies

    private float _radius = 1.5f;
    private float _angle = 135.0f;

    private Transform _player;
    private Vector2 _lookDirection;


    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
    }


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
        SetLookDirection();
        foreach(GameObject enemy in EnemyManager.Instance.GetEnemiesForSwipe(transform.position))
        {
            if(IsInArc(enemy.transform.position))
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


    private bool IsInArc(Vector2 worldPosition)
    {

        Vector2 directionToEnemy = worldPosition - (Vector2)transform.position;

        if(directionToEnemy.sqrMagnitude > _radius * _radius)
        {
            return false;
        }

        // this wont work. 
        float angleToEnemy = Vector2.Angle(transform.right, directionToEnemy);

        return angleToEnemy <= _angle * 0.5f;
    }



    void SetLookDirection()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _lookDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    Vector2 GetPlayerPosition()
    {
        return new Vector2(_player.transform.position.x, _player.transform.position.y);
    }

    private Vector2 GetPointOnArc(float angle)
    {
        

        Vector2 direction =
            Quaternion.Euler(0f, 0f, angle) * _lookDirection;

        return GetPlayerPosition() + direction * _radius;
    }

}
