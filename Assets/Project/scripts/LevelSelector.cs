using System.Collections;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public LevelData[] allLevels;      
    public LevelManager levelManager;
    public ScreenFader fader;
    public float feedbackDelay = 1.5f;

    private int currentLevelIndex = 0;

    void Start()
    {
        levelManager.OnLevelEnded += HandleLevelChange;
        levelManager.OnCorrectMoleHit += HandleLevelChange;
        StartFirstLevel();
    }

    public void StartFirstLevel()
    {
        currentLevelIndex = 0;
        if (allLevels.Length > 0)
            levelManager.StartLevel(allLevels[currentLevelIndex]);
    }

    public void StartNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < allLevels.Length)
        {
            levelManager.StartLevel(allLevels[currentLevelIndex]);
        }
        else
        {
            Debug.Log("¡Todos los niveles completados!");
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
            levelManager.OnCorrectMoleHit -= HandleLevelChange;
        }
    }
}

