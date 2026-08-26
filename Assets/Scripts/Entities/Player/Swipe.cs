using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{

    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private float _lifetime = 0.25f;


    private float dps = 10.0f; // high DPS to combat the short lifetime. A normalization for 0.2s lifetime -
                               // so torch deals 1 damage killing smallest enemies

    private float _radius = 2.5f;
    private float _angle = 135.0f;

    private Vector2 _origin;
    private Vector2 _lookDirection;

    public void Initialize(Vector2 origin, Vector2 lookDirection)
    {
        _origin = origin;
        _lookDirection = lookDirection.normalized;
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
        
        foreach(GameObject enemy in EnemyManager.Instance.GetEnemiesForSwipe(_origin))
        {
            if(IsInArc(enemy.transform.position))
            {
                if (enemy.TryGetComponent(out EntityHealth entityHealth))
                {
                    entityHealth.LoseHealth(dps * Time.deltaTime);
                }
            }
            else
            {
                Debug.Log("Outside arc!");
            }
           
        }

    }

    private Vector2 GetUpperBoundary()
    {
        return Quaternion.Euler(
            0f,
            0f,
            _angle * 0.5f) * _lookDirection;
    }

    private Vector2 GetLowerBoundary()
    {
        return Quaternion.Euler(
            0f,
            0f,
            -_angle * 0.5f) * _lookDirection;
    }


    /*
https://stackoverflow.com/questions/243945/calculating-a-2d-vectors-cross-product
     */
    float Cross(Vector2 v1, Vector2 v2)
    {
        return (v1.x * v2.y) - (v1.y * v2.x);
    }


    private bool IsInArc(Vector2 worldPosition)
    {
        
        Vector2 toEnemy =
            worldPosition - _origin;

        if (toEnemy.sqrMagnitude > _radius * _radius)
        {
            return false;
        }

        if(toEnemy.sqrMagnitude < 0.001f)
        {
            return true;
        }

        toEnemy.Normalize();
        float lowerSide = Cross(GetLowerBoundary(), toEnemy);
        float upperSide = Cross(toEnemy, GetUpperBoundary());

        return lowerSide >= 0f && upperSide >= 0f;
    }



    private Vector2 GetPointOnArc(float angle)
    {
        Vector2 direction =
            Quaternion.Euler(0f, 0f, angle) * _lookDirection;
        return _origin + direction * _radius;
    }

}
