using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;
    [SerializeField] float _healthRegen;

    public Action OnDeath;
    public Action<float, float> OnHealthChanged;

    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    // This seemed to be missing for object pooling script
    void OnEnable()
    {
        ResetHealth();
    }

    void OnDisable()
    {

    }

    void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(HandleHealthRegen), 1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseHealth(float healthLost)
    {
        _currentHealth -= healthLost;
        OnHealthChanged?.Invoke(Mathf.Clamp(_currentHealth, 0, _maxHealth), _maxHealth);

        if(_currentHealth <= 0)
        {
            Death();
        }
    }

    void HandleHealthRegen()
    {
       // _currentHealth = Mathf.Clamp(_currentHealth + _maxHealth * _healthRegen, 0, _maxHealth);
       // OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void Death()
    {
        OnDeath?.Invoke();
    }


}
