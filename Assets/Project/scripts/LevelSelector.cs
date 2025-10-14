using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    
    public LevelData[] MathLevels;      
    public LevelData[] ArtLevels;      
    public LevelData[] CatalanLevels;      
    public LevelData[] SpanishLevels;      
    public LevelManager levelManager;
    public ScreenFader fader;
    public float feedbackDelay = 1.5f;

    private int currentLevelIndex = 0;
    public enum Subject { Math, Art, Catalan, Spanish }
    public Subject currentSubject = Subject.Math;
    private LevelData[] CurrentLevels
    {
        get
        {
            switch (currentSubject)
            {
                case Subject.Math: return MathLevels;
                case Subject.Art: return ArtLevels;
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
            Debug.Log("¡Todos los niveles de " + currentSubject + " completados!");
            ReturnToSubjectMenu();
        }
    }

    public void ChangeSubject(Subject newSubject)
    {
        currentSubject = newSubject;
        currentLevelIndex = 0;
        StartFirstLevel();
    }

    private void ReturnToSubjectMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void HandleLevelChange()
    {
        StartCoroutine(LevelTransitionCoroutine());
    }

    private IEnumerator LevelTransitionCoroutine()
    {
        yield return new WaitForSeconds(feedbackDelay);

        if (fader != null)
            yield return fader.FadeOut();

        StartNextLevel();

        if (fader != null)
            yield return fader.FadeIn();
    }

    void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelEnded -= HandleLevelChange;
        }
    }
}

