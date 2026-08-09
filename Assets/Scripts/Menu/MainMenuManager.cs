using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] CanvasGroup _mainMenuButtonsCG;
    [SerializeField] CanvasGroup _quitConfirmCG;
    CanvasGroup _mainMenuCG;
    [SerializeField] CanvasGroup _settingsMenuCG;
    [SerializeField] CanvasGroup _fadeOverlay;
    float _fadeDuration = 12f;

    void Awake()
    {
        _mainMenuCG = GetComponent<CanvasGroup>();
        CanvasGroupSetState(_mainMenuCG, false);
    }

    private void Start()
    {

        StartCoroutine(FadeFromBlack());
    }

    IEnumerator FadeFromBlack()
    {

        yield return new WaitForSecondsRealtime(1.5f);

        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            _fadeOverlay.alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / _fadeDuration);

            yield return null;
        }

        _fadeOverlay.alpha = 0f;
    }


    public void OpenMainMenu()
    {
        CanvasGroupSetState(_mainMenuCG, true);
    }

    public IEnumerator CloseMainMenu(float duration)
    {

        float elapsed = 0f;
        _mainMenuCG.interactable = false;
        _mainMenuCG.blocksRaycasts = false;
        float startingAlpha = _mainMenuCG.alpha;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            _mainMenuCG.alpha =
                Mathf.Lerp(startingAlpha, 0f, elapsed / duration);

            yield return null;
        }

        _mainMenuCG.alpha = 0f;

    }



    public void Play()
    {
        GameManager.Instance.OnNewGameRequested();
    }



    void CanvasGroupSetState(CanvasGroup canvasGroup, bool state)
    {
        canvasGroup.alpha = state ? 1.0f : 0.0f;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    public void OpenQuitConfirmation()
    {
        CanvasGroupSetState(_mainMenuButtonsCG, false);
        CanvasGroupSetState(_quitConfirmCG, true);
    }


    public void CloseQuitConfirmation()
    {
        CanvasGroupSetState(_quitConfirmCG, false);
        CanvasGroupSetState(_mainMenuButtonsCG, true);
    }

    public void SettingsMenuToggle(bool open)
    {
        CanvasGroupSetState(_mainMenuButtonsCG, !open);
        CanvasGroupSetState(_settingsMenuCG, open);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Succesful app shutdown");
#endif
    }
}
