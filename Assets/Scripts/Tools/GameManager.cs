using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    InMenu,
    Playing,
    Countdown,
    Ending,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] InGameUIManager _inGameUIManager;
    [SerializeField] EnemyManager _enemyManager;
    [SerializeField] Timer timer;
    [SerializeField] Player player;
    public static GameManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameState State { get; private set; }

    int kills = 0;
    double hits = 0;
    double misses = 0;
    private float _spawnAccumulator;

    private float _countDownSpawnRate = 90f;

    [SerializeField] MainMenuManager _mainMenuManager;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 0f;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _enemyManager = FindFirstObjectByType<EnemyManager>();
        _inGameUIManager = FindFirstObjectByType<InGameUIManager>();
        timer = FindFirstObjectByType<Timer>();
    }


    void Start()
    {
        State = GameState.InMenu;
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
        timer.StartTimer();
        State = GameState.InMenu;

    }

    public void StopGameTimer()
    {
        timer.StopTimer();
    }

    public void SetUI(InGameUIManager ui)
    {
        _inGameUIManager = ui;
    }

    public void StartGame()
    {


        // apply countdown of three seconds, then apply timescale and then start playing song
        Time.timeScale = 1f;
        AudioManager.Instance.StopMusic();
        State = GameState.Countdown;
        StartCoroutine(StartRoundCountdown());
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (State != GameState.Playing) return;
        _inGameUIManager.updateTimerText(timer.ElapsedTime);
          
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        double accP = hits / (hits + misses);

        _inGameUIManager.ShowGameOverPanel(timer.ElapsedTime, kills, accP);
    }

    private IEnumerator StartRoundCountdown()
    {
      
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayCountDown();
        _inGameUIManager.updateTimerText(0.0f);
        _inGameUIManager.setCountdownText("3");
        yield return new WaitForSecondsRealtime(1f);
       
        _inGameUIManager.setCountdownText("2");
        yield return new WaitForSecondsRealtime(1f);

        _inGameUIManager.setCountdownText("1");
        yield return new WaitForSecondsRealtime(1f);
        _inGameUIManager.setCountdownText("GO");

        yield return new WaitForSecondsRealtime(1f);
        _inGameUIManager.setCountdownText("");
        AudioManager.Instance.PlayMusic();
        SetState(GameState.Playing);
        _enemyManager.OnStartGame();
        timer.StartTimer();
    }



    public void SetGameEndsNow()
    {
        State = GameState.Ending;
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayOnPlayerDeath();

    }

    public Player GetPlayerReference()
    {
        return player;
    }


    public void SetState(GameState state)
    {
        State = state;
    }

    public GameState GetState()
    {
        return State;
    }
}
