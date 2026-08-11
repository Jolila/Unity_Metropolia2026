using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class HealthBar : MonoBehaviour
{

    [SerializeField] PlayerHealthSystem playerHealth;

    [SerializeField] CanvasGroup _normalBar;
    [SerializeField] CanvasGroup _underdriveBar;
    [SerializeField] CanvasGroup _overdriveBar;

    [SerializeField] Image _normalHpBarFill;
    [SerializeField] Image _underdriveHpBarFill;
    [SerializeField] Image _overdriveHpBarFill;

    HealthState _currentHealthState;

    [SerializeField] Image _transitionFlash;
    Color NormalToOverdrive = new Color(0.6f, 0.0f, 0.0f, 0.7f);
    Color OverdriveToNormal = new Color(0.4f, 0.4f, 0.4f, 0.3f);
    Color NormalToUnderdrive = new Color(0.4f, 0.4f, 0.4f, 0.3f);
    Color UnderDriveToNormal = new Color(0.6f, 0.0f, 0.0f, 0.5f);


    void OnEnable()
    {
        playerHealth.OnHealthStateChanged += HandleHealthStateChanged;
    }

    void OnDisable()
    {
        playerHealth.OnHealthStateChanged -= HandleHealthStateChanged;
    }
    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealthSystem>();
    
        
        _transitionFlash.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        _currentHealthState = playerHealth.CurrentHealthState;
        SetHealthBar(_currentHealthState, 1f);
    }

    // Update is called once per frame
    void Update()
    {

        UpdateHealthFill();
        

    }

    private void UpdateHealthFill()
    {
        float fillAmount = playerHealth.CurrentHealth / playerHealth.CurrentMaxHealth;

        switch (_currentHealthState)
        {
            case HealthState.Normal:
                _normalHpBarFill.fillAmount = fillAmount;
                break;

            case HealthState.Underdrive:
                _underdriveHpBarFill.fillAmount = fillAmount;
                break;

            case HealthState.Overdrive:
                _overdriveHpBarFill.fillAmount = fillAmount;
                break;
        }
    }

    private void HandleHealthStateChanged(HealthState newState)
    {
        HealthState previousState = _currentHealthState;

        _currentHealthState = newState;

        PlayTransition(previousState, newState);
        SetHealthBar(previousState, 0);
        SetHealthBar(newState, 1f);
    }

    private void PlayTransition(
       HealthState previousState,
       HealthState newState)
    {

        if (previousState == newState) return;

        if (previousState == HealthState.Normal &&
            newState == HealthState.Overdrive)
        {
            StartCoroutine(PlayFlash(NormalToOverdrive));
            return;
        }

        if (previousState == HealthState.Overdrive &&
            newState == HealthState.Normal)
        {
            StartCoroutine(PlayFlash(OverdriveToNormal));
            return;
        }

        if (previousState == HealthState.Normal &&
            newState == HealthState.Underdrive)
        {
            StartCoroutine(PlayFlash(NormalToUnderdrive));
            return;
        }
        if(previousState == HealthState.Underdrive &&
            newState == HealthState.Normal)
        {
            StartCoroutine(PlayFlash(UnderDriveToNormal));
            return;
        }
        
    }


    private void SetHealthBar(HealthState state, float alpha)
    {
       
        switch (state)
        {
            case HealthState.Normal:
                _normalBar.alpha = alpha;
                break;

            case HealthState.Underdrive:
                _underdriveBar.alpha = alpha;
                break;

            case HealthState.Overdrive:
                _overdriveBar.alpha = alpha;
                break;
        }
    }




    private IEnumerator PlayFlash(Color flashColor)
    {
        _transitionFlash.color = flashColor;

        yield return new WaitForSeconds(0.08f);

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                flashColor.a,
                0f,
                elapsed / duration
            );

            _transitionFlash.color =
                new Color(
                    flashColor.r,
                    flashColor.g,
                    flashColor.b,
                    alpha
                );

            yield return null;
        }

        _transitionFlash.color =
            new Color(
                flashColor.r,
                flashColor.g,
                flashColor.b,
                0f
            );
    }


}
