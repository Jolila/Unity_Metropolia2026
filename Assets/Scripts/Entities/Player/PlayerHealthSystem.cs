using System;
using System.Collections;
using UnityEngine;


public enum HealthState
{
    Underdrive, Normal, Overdrive
}

public class PlayerHealthSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float NormalMaxHealth = 100f;
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

    float UnderdriveThreshHold = 0.2f;
    float OverdriveTreshHold = 0.85f;
    float OverdriveToNormalThreshHold = 0.4f;

    bool IsDead;
    public Action OnPlayerDeath;

    public Action<HealthState> OnHealthStateChanged;
    public HealthState CurrentHealthState { get; private set; }

    private static PlayerHealthSystem _instance;
    public static PlayerHealthSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerHealthSystem>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no enemy manager instance");
                }
            }
            return _instance;
        }
    }

    
    void Start()
    {
        _currentHealth = 40f;
        _currentMaxHealth = NormalMaxHealth;
        CurrentHealthState = HealthState.Normal;
    }

    private void EvaluateHealthState()
    {
        float healthPercentage = CurrentHealth / NormalMaxHealth;
        HealthState newState = CurrentHealthState;

        switch(CurrentHealthState)
        {
            case HealthState.Normal:
                if (healthPercentage <= UnderdriveThreshHold) newState = HealthState.Underdrive;
                else if (healthPercentage >= OverdriveTreshHold) newState = HealthState.Overdrive;
                break;

            case HealthState.Underdrive:
                if (healthPercentage > UnderdriveThreshHold) newState = HealthState.Normal;
                break;
            case HealthState.Overdrive:
                if (healthPercentage <=  OverdriveToNormalThreshHold) newState = HealthState.Normal;
                break;
        }

        if (newState == CurrentHealthState) return;
        
        CurrentHealthState = newState;
        UpdateMaxHealthForState();
        OnHealthStateChanged?.Invoke(CurrentHealthState);

    }

    private void UpdateMaxHealthForState()
    {
        switch (CurrentHealthState)
        {
            case HealthState.Normal:
            case HealthState.Underdrive:
                _currentMaxHealth = 100f;
                break;

            case HealthState.Overdrive:
                _currentMaxHealth = 300f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {


        // check if player is colliding with blood droplets and heal accordingly
        // check for bloodstarved and overdrive and upstream events for subsystems

        if (GameManager.Instance.GetState() != GameState.Playing) return;
        DevInputFunctions();
        EvaluateHealthState();
        HandleAmbientDamage();
        HandleContactDamage();



        if (CurrentHealth <= 0f)
        {
            Die();
        }

       
    }

    public void DevInputFunctions()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            _currentHealth -= 10f;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentHealth += 15f;
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

    public void Heal(float value)
    {
        _currentHealth += value;
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
