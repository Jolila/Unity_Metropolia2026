using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    CanvasGroup _cg;
    [SerializeField] CanvasGroup _gameOverPanelCG;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void ShowInGameUI()
    {
        _cg.alpha = 1.0f;
        _cg.interactable = true;
        _cg.blocksRaycasts = true;
    }

    public void ShowGameOverPanel()
    {
        _gameOverPanelCG.alpha = 1;
        _gameOverPanelCG.interactable = true;
        _gameOverPanelCG.blocksRaycasts = true;

        _cg.alpha = 0;
      

    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ResetGame();
    }
}
