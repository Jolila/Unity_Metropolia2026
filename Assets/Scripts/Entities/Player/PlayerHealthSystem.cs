using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float DefaultMaxHealth = 100f;
    float MaxHealthOnOverDrive = 300f;
    float _currentHealth;
    float _currentMaxHealth;

    public float CurrentMaxHealth => _currentMaxHealth;
    public float CurrentHealth => _currentHealth;

   
    float damageRadius = 0.4f;
    float contactDPS = 1.0f;

    float ambientBloodLoss = 1.0f;
    float ambientBloodLossOnOverdrive = 2f;
    float BloodLostOnFireRing = 66.6f;
    float BloodLostOnShot = 1f;
    float BloodLostOnShotOnOverdrive = 2f;

    bool IsDead;
    public Action OnPlayerDeath;

    public Action OnOverDriveStarted;
    public Action OnOverDriveEnded;

    public Action OnBloodStarvedStarted;
    public Action OnBloodStarvedEnded;


    void Start()
    {
        _currentHealth = 40f;
        _currentMaxHealth = DefaultMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {


        // check if player is colliding with blood droplets and heal accordingly
        // check for bloodstarved and overdrive and upstream events for subsystems

        if (GameManager.Instance.GetState() != GameState.Playing) return;
        HandleAmbientDamage();
        HandleContactDamage();

        if (CurrentHealth <= 0f)
        {
            Die();
        }

       
    }

    void HandleAmbientDamage()
    {
        _currentHealth -= ambientBloodLoss * Time.deltaTime;
    }

    void HandleContactDamage()
    {


        int touching = 0;
        float radiusSq = damageRadius * damageRadius;

        foreach (GameObject enemy in EnemyManager.Instance.GetEnemiesInCell(
                    transform.position))
        {
            if (!enemy.activeInHierarchy)
                continue;

            if ((enemy.transform.position - transform.position).sqrMagnitude
                <= radiusSq)
            {
                touching++;
            }
        }

        if (touching > 0)
        {
            _currentHealth -= touching * contactDPS * Time.deltaTime;
        }
    }


    private void Die()
    {
        IsDead = true;
        OnPlayerDeath?.Invoke();
        GameManager.Instance.StopGameTimer();
        GameManager.Instance.SetGameEndsNow();
        StartCoroutine(WaitAndExecute());
    }

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.GameOver();
    }
}
