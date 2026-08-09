using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;
    [SerializeField] float _healthRegen;
    public Action<float, float> OnHealthChanged;
    public Action OnDeath;
    public Action OnDeSpawn;
    public bool _isDead;
        
    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    // This seemed to be missing for object pooling script
    void OnEnable()
    {
        ResetHealth();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        _isDead = false;
    }

    void OnDisable()
    {

    }

    void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }


    public void LoseHealth(float healthLost)
    {
        if (_isDead) return;
        _currentHealth -= healthLost;
        OnHealthChanged?.Invoke(Mathf.Clamp(_currentHealth, 0, _maxHealth), _maxHealth);

        if(_currentHealth <= 0)
        {
            _isDead = true;
            Death();
        }
    }



    public void Death()
    {
        OnDeath?.Invoke();
    }

    public void Despawn()
    {
        OnDeSpawn?.Invoke();
    }


}
