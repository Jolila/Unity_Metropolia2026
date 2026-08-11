using UnityEngine;
using UnityEngine.UI;


public enum HealthBarMode
{
    Normal,Underdrive,Overdrive
}

public class HealthBar : MonoBehaviour
{

    [SerializeField] PlayerHealthSystem playerHealth;


    [SerializeField] CanvasGroup _normalBar;
    [SerializeField] CanvasGroup _underdriveBar;
    [SerializeField] CanvasGroup _overdriveBar;

    [SerializeField] Image _normalHpBarFill;
    [SerializeField] Image _underdriveHpBarFill;
    [SerializeField] Image _overdriveHpBarFill;

    HealthBarMode _currentHealtMode;

    private void Awake()
    {
        Debug.Log($"NORMAL: {_normalBar}");
        Debug.Log($"UNDERDRIVE: {_underdriveBar}");
        Debug.Log($"OVERDRIVE: {_overdriveBar}");
    }


    void OnEnable()
    {
        
    }

    void OnDisable()
    {
       
    }
    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealthSystem>();
        Debug.Log($"HealthBar instance: {gameObject.name}");
        Debug.Log($"NormalBar: {_normalBar}");
        Debug.Log($"UnderdriveBar: {_underdriveBar}");
        Debug.Log($"OverdriveBar: {_overdriveBar}");
        Debug.Log($"PlayerHealth: {playerHealth}");

       
        _currentHealtMode = HealthBarMode.Normal;
        SetHealthBarMode(_currentHealtMode);
    }

    // Update is called once per frame
    void Update()
    {

        TestInput();
        UpdateHealthFill();
        

    }

    private void UpdateHealthFill()
    {
        float fillAmount = playerHealth.CurrentHealth / playerHealth.CurrentMaxHealth;

        switch (_currentHealtMode)
        {
            case HealthBarMode.Normal:
                _normalHpBarFill.fillAmount = fillAmount;
                break;

            case HealthBarMode.Underdrive:
                _underdriveHpBarFill.fillAmount = fillAmount;
                break;

            case HealthBarMode.Overdrive:
                _overdriveHpBarFill.fillAmount = fillAmount;
                break;
        }
    }

    private void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetHealthBarMode(HealthBarMode.Normal);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetHealthBarMode(HealthBarMode.Underdrive);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetHealthBarMode(HealthBarMode.Overdrive);
        }
    }


    private void SetHealthBarMode(HealthBarMode newMode)
    {
        _currentHealtMode = newMode;

        _normalBar.alpha = newMode == HealthBarMode.Normal ? 1f : 0f;
        _underdriveBar.alpha = newMode == HealthBarMode.Underdrive ? 1f : 0f;
        _overdriveBar.alpha = newMode == HealthBarMode.Overdrive ? 1f : 0f;
    }

   
}
