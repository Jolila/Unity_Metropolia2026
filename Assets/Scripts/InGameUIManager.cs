using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    
    [SerializeField] CanvasGroup _gameOverPanelCG;
    [SerializeField] CanvasGroup _inGameCanvasGroup;

    private void Awake()
    {
    }

    public void ShowInGameUI()
    {
        _inGameCanvasGroup.alpha = 1.0f;
        _inGameCanvasGroup.interactable = true;
        _inGameCanvasGroup.blocksRaycasts = true;
    }

    public void ShowGameOverPanel()
    {
        _gameOverPanelCG.alpha = 1;
        _gameOverPanelCG.interactable = true;
        _gameOverPanelCG.blocksRaycasts = true;

        _inGameCanvasGroup.alpha = 0;
        _inGameCanvasGroup.interactable = false;
        _inGameCanvasGroup.blocksRaycasts = false;

    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ResetGame();
    }
}
