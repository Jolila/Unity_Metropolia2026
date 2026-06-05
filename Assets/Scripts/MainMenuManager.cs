using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] CanvasGroup _mainMenuButtonsCG;
    [SerializeField] CanvasGroup _quitConfirmCG;
    CanvasGroup _mainMenuCG;

    void Awake()
    {
        _mainMenuCG = GetComponent<CanvasGroup>();
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        CanvasGroupSetState(_mainMenuCG, true);
    }

    public void CloseMainMenu()
    {
        CanvasGroupSetState(_mainMenuCG, false);
    }

    public void Play()
    {
        CloseMainMenu();
        GameManager.Instance.StartGame();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
