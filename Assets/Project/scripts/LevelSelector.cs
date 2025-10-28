using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private LevelData[] MathLevels;
    [SerializeField] private LevelData[] EnglishLevels;      
    [SerializeField] private LevelData[] CatalanLevels;      
    [SerializeField] private LevelData[] SpanishLevels;
    public enum Subject { Math, English, Catalan, Spanish }

    [SerializeField] private Subject currentSubject = Subject.Math;
    [Header("Manager")]
    [SerializeField] private LevelManager levelManager;

    [Header("Fader")]
    [SerializeField] private ScreenFader fader;
    [SerializeField] private float feedbackDelay = 1.5f;

    private int currentLevelIndex = 0;
    
    private LevelData[] CurrentLevels
    {
        get
        {
            switch (currentSubject)
            {
                case Subject.Math: return MathLevels;
                case Subject.English: return EnglishLevels;
                case Subject.Catalan: return CatalanLevels;
                case Subject.Spanish: return SpanishLevels;
                default: return null;
            }
        }
    }

    void Start()
    {
        currentSubject = GameManager.Instance.selectedSubject;

        levelManager.OnLevelEnded += HandleLevelChange;
        StartFirstLevel();
    }

    public void StartFirstLevel()
    {
        currentLevelIndex = 0;
        var levels = CurrentLevels;
        if (levels != null && levels.Length > 0)
            levelManager.StartLevel(levels[currentLevelIndex]);
    }

    public void StartNextLevel()
    {
        currentLevelIndex++;
        var levels = CurrentLevels;
        if (levels != null && currentLevelIndex < levels.Length)
        {
            levelManager.StartLevel(levels[currentLevelIndex]);
        }
        else
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddTempToGlobal();
            }

            FinishSubject();
        }
    }

    public void ChangeSubject(Subject newSubject)
    {
        currentSubject = newSubject;
        currentLevelIndex = 0;
        StartFirstLevel();
    }

    public void RestartCurrentLevel()
    {
        var levels = CurrentLevels;
        if (levels != null && currentLevelIndex < levels.Length)
        {
            levelManager.RestartCurrentLevel(levels[currentLevelIndex]);
        }
    }

    private void HandleLevelChange()
    {
        StartCoroutine(LevelTransitionCoroutine());
    }

    private IEnumerator LevelTransitionCoroutine()
    {
        yield return new WaitForSeconds(feedbackDelay);

        if (fader != null)
            yield return fader.FadeOutIn(StartNextLevel);
        else
            StartNextLevel();
    }

    public void FinishSubject()
    {
        ScoreManager.Instance.AddTempToGlobal();
        ScoreManager.Instance.ResetTempScore();

        SceneManager.LoadScene("SubjectMenuScene");
        MusicManager.Instance.PlayLastMenuMusic();
    }

    public void ReturnButton()
    {
        ClickSoundManager.Instance.PlayClick();
        MusicManager.Instance.PlayLastMenuMusic();

        ScoreManager.Instance.AddTempToGlobal();
        ScoreManager.Instance.ResetTempScore();

        SceneManager.LoadScene("SubjectMenuScene");
    }

    void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelEnded -= HandleLevelChange;
        }
    }
}

