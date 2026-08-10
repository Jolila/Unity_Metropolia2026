using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image _hpBarFill;
    [SerializeField] PlayerHealthSystem playerHealth;


    void OnEnable()
    {
        playerHealth = FindAnyObjectByType<PlayerHealthSystem>();
    }

    void OnDisable()
    {
       
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //_hpBarFill.fillAmount = playerHealth.CurrentHealth / playerHealth.CurrentMaxHealth;
    }

   
}
