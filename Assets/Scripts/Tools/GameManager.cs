using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] InGameUIManager _inGameUIManager;
    public static GameManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
        
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        _inGameUIManager.ShowGameOverPanel();
    }
}
