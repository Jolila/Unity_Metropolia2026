using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    
    [SerializeField] CanvasGroup _gameOverPanelCG;
    [SerializeField] CanvasGroup _inGameCanvasGroup;

    [SerializeField] private TMP_Text InGameTimerText;

    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text killCountText;

    private void Awake()
    {

    }

    private void Update()
    {
        float time = Timer.Instance.ElapsedTime;

        int minutes = (int)time / 60;
        int sec = (int)time % 60;

        InGameTimerText.text = $"Time: {minutes:00}:{sec:00}";
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
        // Update the timer postscreen, and killcount here

        float time = Timer.Instance.ElapsedTime;

        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        int 

        finalTimeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ResetGame();
        Timer.Instance.Reset();
        // set the timer singleton state to 0.0
    }
}
