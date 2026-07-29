using UnityEngine;
using TMPro;
using System;


public class Timer : MonoBehaviour
{


    private static Timer _instance;
    public static Timer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<Timer>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no timer instance");
                }
            }
            return _instance;
        }
    }




    [SerializeField] private TMP_Text textbox;
    [SerializeField] GameObject player;
    private EntityHealth playerHealth;
    private float elapsedTime;
    private bool isRunning = true;

    public float ElapsedTime => elapsedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

        playerHealth = player.GetComponent<EntityHealth>();
        playerHealth.OnDeath += StopTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            elapsedTime += Time.deltaTime;
        }

    }

    void StopTimer()
    {
        isRunning = false;
    }


    public void Reset()
    {
        elapsedTime = 0.0f;
        isRunning = true;
    }


}
