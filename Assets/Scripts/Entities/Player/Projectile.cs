
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float _travelSpeed;
    [SerializeField] float _damage;
    Vector2 direction;
    Vector2 position;
    float radius = 0.075f; // updated value to match new visual

    public void InitializeProjectile(Vector3 spawnPoint, Vector2 dir)
    {
        gameObject.SetActive(true);
        transform.position = spawnPoint;
        direction = dir.normalized;
    }




    private void Update()
    {
        float distance = _travelSpeed * Time.deltaTime;

        transform.position += (Vector3)(direction * distance);

        if (LevelLoader.Instance.CurrentLevel.IsWall(transform.position))
        {
            DestroyProjectileOnTerrain();
            return;
        }
        
        foreach (GameObject enemy in EnemyManager.Instance.GetEnemiesInCell(
            transform.position))
        {
            if ((enemy.transform.position - transform.position).sqrMagnitude < radius)
            {
                DealDamage(enemy);

                enemy.transform.position +=
                    (Vector3)direction * 0.15f;

                DestroyProjectileOnEnemy();
                return;
            }
        }


    }


    void DealDamage(GameObject target)
    {
        if(target.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.LoseHealth(_damage);
            AudioManager.Instance.PlayEnemyHit();
        }
    }


   

    void DestroyProjectileOnTerrain()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RegisterMiss();
    }

    void DestroyProjectileOnEnemy()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RegisterHit();
    }


}
