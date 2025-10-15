using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreTextTemporary;
    public TextMeshProUGUI scoreTextGlobal;

    private int tempScore = 0;
    private int globalScore = 0;

    public int TempScore => tempScore;
    public int GlobalScore => globalScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scoreTextGlobal == null)
        {
            var textObj = GameObject.FindWithTag("GlobalScoreText");
            if (textObj != null)
            {
                scoreTextGlobal = textObj.GetComponent<TextMeshProUGUI>();
                UpdateGlobalUI();
            }
        }

        if (scoreTextTemporary == null)
        {
            var obj = GameObject.FindWithTag("TempScoreText");
            if (obj != null)
            {
                scoreTextTemporary = obj.GetComponent<TextMeshProUGUI>();
                UpdateTempUI();
            }
        }
    }

    public void AddTempScore(int amount)
    {
        tempScore += amount;
        UpdateTempUI();
    }

    public void SubtractTempScore(int amount)
    {
        tempScore -= amount;
        if (tempScore < 0) tempScore = 0;
        UpdateTempUI();
    }

    public void ResetTempScore()
    {
        tempScore = 0;
        UpdateTempUI();
    }
    public void SetTempScore(int value)
    {
        tempScore = value;
        UpdateTempUI();
    }

    private void UpdateTempUI()
    {
        if (scoreTextTemporary != null)
            scoreTextTemporary.text = "PUNTUACIÓ: " + tempScore;
    }

    public void AddTempToGlobal()
    {
        globalScore += tempScore;
        tempScore = 0;
        UpdateGlobalUI();
        UpdateTempUI();
    }

    public void ResetGlobalScore()
    {
        globalScore = 0;
        tempScore = 0;
        UpdateGlobalUI();
        UpdateTempUI();
    }

    private void UpdateGlobalUI()
    {
        if (scoreTextGlobal != null)
            scoreTextGlobal.text = "PUNTUACIÓ: " + globalScore;
    }

    public int GetGlobalScore() => globalScore;
}
