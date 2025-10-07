using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public LevelData[] allLevels;      
    public LevelManager levelManager;  

    private int currentLevelIndex = 0;

    void Start()
    {
        levelManager.OnLevelEnded += StartNextLevel;
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
}

