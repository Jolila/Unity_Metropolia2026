using System.Collections;
using UnityEngine;

public class TransitionsManager : MonoBehaviour
{
    public static TransitionsManager Instance { get; private set; }

    [SerializeField] private CanvasGroup _bloodMoonOverlay;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeToBloodMoon(float duration)
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeFromBloodMoon(float duration)
    {
        yield return Fade(1f, 0f, duration);
    }


    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            _bloodMoonOverlay.alpha =
                Mathf.Lerp(from, to, elapsed / duration);

            yield return null;
        }

        _bloodMoonOverlay.alpha = to;
    }
}
