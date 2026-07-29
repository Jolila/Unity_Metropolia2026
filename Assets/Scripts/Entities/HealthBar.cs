using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image _hpBarFill;
    [SerializeField] EntityHealth entityHealth;


    void OnEnable()
    {
        entityHealth.OnHealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        entityHealth.OnHealthChanged -= OnHealthChanged;
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
