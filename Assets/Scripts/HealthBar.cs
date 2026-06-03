using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image _hpBarFill;
    [SerializeField] EntityHealth _playerHealth;


    void OnEnable()
    {
        _playerHealth.OnHealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        _playerHealth.OnHealthChanged -= OnHealthChanged;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        _hpBarFill.fillAmount = currentHealth / maxHealth;
    }
}
