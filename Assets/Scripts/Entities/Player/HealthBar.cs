using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] HealthBar _hpBarFill;
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
        RectTransform rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        _hpBarFill.fillAmount = playerHealth.CurrentHealth / playerHealth.CurrentMaxHealth;
    }

   
}
