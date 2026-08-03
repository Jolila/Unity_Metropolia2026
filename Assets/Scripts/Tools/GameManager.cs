using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] InGameUIManager _inGameUIManager;
    [SerializeField] Timer timer;
    public static GameManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    int kills = 0;
    double hits = 0;
    double misses = 0;

    [SerializeField] MainMenuManager _mainMenuManager;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 0f;
    }
    void Start()
    {
        
    }

    public void RegisterMiss()
    {
        ++misses;
    }

    public void RegisterHit()
    {
        ++hits;
    }

    public void RegisterKill()
    {
        ++kills;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
        kills = 0;
        hits = 0;
        misses = 0;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(_inGameUIManager != null) _inGameUIManager.updateTimerText(timer.ElapsedTime);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        double accP = hits / (hits + misses);

        _inGameUIManager.ShowGameOverPanel(timer.ElapsedTime, kills, accP);
    }
}
