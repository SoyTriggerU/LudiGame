using UnityEngine;
using TMPro;
using System;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;
    public bool isRunning { get; private set; }
    [SerializeField] private bool isPaused = false;

    public event Action OnTimerEnded;

    public void StartTimer(float duration)
    {
        currentTime = duration;
        isRunning = true;
        UpdateUI();
    }

    void Update()
    {
       if (!isRunning || isPaused) return;

        currentTime -= Time.deltaTime; 

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimerEnded?.Invoke(); 
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = ("TEMPS RESTANT: " + Mathf.CeilToInt(currentTime).ToString()); 
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }
}
