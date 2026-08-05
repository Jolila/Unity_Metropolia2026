using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    
    [SerializeField] CanvasGroup _gameOverPanelCG;
    [SerializeField] CanvasGroup _inGameCanvasGroup;

    [SerializeField] private TMP_Text InGameTimerText;
    [SerializeField] private TMP_Text CountdownText;

    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] TMP_Text accuracyText;
    [SerializeField] PostGameScreenController _postScreenAnimator;

    private void Awake()
    {
   
    }

    private void Update()
    {

    }

    private void Start()
    {
        GameManager.Instance.SetUI(this);
    }

    public void updateTimerText(float time)
    {
        
        int minutes = (int)time / 60;
        int sec = (int)time % 60;

        InGameTimerText.text = $"Time: {minutes:00}:{sec:00}";
    }

    public void setCountdownText(string text)
    {
        CountdownText.text = text;
    }

    public void ShowInGameUI()
    {
        _inGameCanvasGroup.alpha = 1.0f;
        _inGameCanvasGroup.interactable = true;
        _inGameCanvasGroup.blocksRaycasts = true;
    }

    public void ShowGameOverPanel(float finaltime, int finalKills, double accuracyP)
    {
        _gameOverPanelCG.alpha = 1;
        _gameOverPanelCG.interactable = true;
        _gameOverPanelCG.blocksRaycasts = true;

        _inGameCanvasGroup.alpha = 0;
        _inGameCanvasGroup.interactable = false;
        _inGameCanvasGroup.blocksRaycasts = false;
        // Update the timer postscreen, and killcount here

        int minutes = (int)finaltime / 60;
        int seconds = (int)finaltime % 60;
        int h = (int)((finaltime % 60f - seconds) * 100f);

        finalTimeText.text = $"Time: {minutes:00}:{seconds:00}:{h:000}";
        killCountText.text = "Kills : " + finalKills;

        double acc = accuracyP * 100;
        string accuracyString;

        if (acc == double.NaN) accuracyString = $"Accuracy : 0 %";
        else if (acc < 10) accuracyString = $"Accuracy : {acc:0}%";
        else accuracyString = $"Accuracy : {acc:00}%";
        accuracyText.text = accuracyString;

        _postScreenAnimator.PrepareAnimation();
        StartCoroutine(_postScreenAnimator.PlayAnimations());


    }

    public void ReturnToMainMenu()
    {
        
        GameManager.Instance.ResetGame();
       
    }
}
