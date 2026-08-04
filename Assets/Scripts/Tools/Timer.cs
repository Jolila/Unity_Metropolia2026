using UnityEngine;
using TMPro;
using System;


public class Timer : MonoBehaviour
{


    [SerializeField] private TMP_Text textbox;
    [SerializeField] GameObject player;
    private EntityHealth playerHealth;
    private float elapsedTime;
    private bool isRunning = true;

    public float ElapsedTime => elapsedTime;

    void Update()
    {
        if(isRunning)
        {
            elapsedTime += Time.deltaTime;
        }

    }

    public void StopTimer()
    {
        isRunning = false;
    }


    public void StartTimer()
    {
        elapsedTime = 0.0f;
        isRunning = true;
    }


}
