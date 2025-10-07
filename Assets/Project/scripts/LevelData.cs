using UnityEngine;

public enum SpawnMode { Sequential, Random }

[CreateAssetMenu(menuName = "Moles/LevelData", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string subject;
    public SpawnMode spawnMode = SpawnMode.Sequential;

    [Header("Moles / spawn")]
    public int moleCount = 4;             
    public float levelTime = 30f;
    public float visibleTime = 1.2f;
    public float riseDuration = 0.25f;
    public float riseDistance = 120f;
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    public int maxConcurrentVisible = 2;

    [Header("Sprites (opciones)")]
    public Sprite[] contentsSprites;        
    public int[] correctSpriteIndices; 
}
