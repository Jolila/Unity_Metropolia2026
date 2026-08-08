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
    [SerializeField] LevelLoader _loader;
    public static GameManager Instance { get; private set; }
    bool startGameOnSceneLoad = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameState State { get; private set; }

    int kills = 0;
    double hits = 0;
    double misses = 0;


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
        State = GameState.InMenu;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator StartNewGame()
    {
        kills = 0;
        hits = 0;
        misses = 0;
        startGameOnSceneLoad = true;
        // querying the previous state here, a bit smelly
        if (State == GameState.InMenu)
        {
            yield return TransitionsManager.Instance.FadeToBloodMoon(0.5f);
            yield return _mainMenuManager.CloseMainMenu(0.5f);
        }
        else if (State == GameState.GameOver)
        {
            yield return TransitionsManager.Instance.FadeToBloodMoon(0.5f);
            yield return _inGameUIManager.HideGameOverPanel(0.5f);
        }

        
        SceneManager.LoadScene(0);
    }

    private IEnumerator InitializeLevel()
    {
        
        yield return StartCoroutine(_loader.LoadLevel());
        
        _enemyManager.Initialize();
       
        if (startGameOnSceneLoad)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            yield return TransitionsManager.Instance.FadeFromBloodMoon(1.0f);
            StartGame();
            startGameOnSceneLoad = false;

        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

       
        player = FindAnyObjectByType<Player>();
        _loader = FindAnyObjectByType<LevelLoader>();
        _inGameUIManager = FindAnyObjectByType<InGameUIManager>();
        _mainMenuManager = FindAnyObjectByType<MainMenuManager>();
        timer = FindAnyObjectByType<Timer>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
        StartCoroutine(InitializeLevel());

    }



    // On application start, the gamestate is in menu
    void Start()
    {
        State = GameState.InMenu;
        _mainMenuManager.OpenMainMenu();
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
        State = GameState.GameOver;
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

    public void OnNewGameRequested()
    {

        StartCoroutine(StartNewGame());
    }

    public void OnMainMenuRequested()
    {
        State = GameState.InMenu;
        _inGameUIManager = FindAnyObjectByType<InGameUIManager>();
        _mainMenuManager.OpenMainMenu();
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
