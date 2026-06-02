using UnityEngine;
using TMPro;
using System;


public class TimerUpdater : MonoBehaviour
{

    [SerializeField] private TMP_Text textbox;
    [SerializeField] GameObject player;
    private EntityHealth playerHealth;
    private double time;
    private int minutes;
    private int seconds;
    private bool timerRunning = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = player.GetComponent<EntityHealth>();
        playerHealth.OnDeath += StopTimer;
        time = 0.0;
        minutes = 0;
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerRunning)
        {
            time += Time.deltaTime;
        }
        minutes = (int)time / 60;
        seconds = (int)time % 60;
        textbox.text = $"Timer: {minutes:00}:{seconds:00}";
    }

    void StopTimer()
    {
        timerRunning = false;
    }


}
