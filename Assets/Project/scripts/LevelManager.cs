using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public RectTransform[] holeTransforms;
    public GameObject molePrefab;
    public Transform moleParent;
    public Text timerText;
    public Text scoreText;

    List<MoleUI> moles = new List<MoleUI>();

    private LevelData levelData;
    float levelTimer;
    int score = 0;
    bool running = false;
    Coroutine spawnRoutine;

    public System.Action OnLevelEnded;

    public void StartLevel(LevelData data)
    {
        levelData = data;
        PrepareMoles();
        levelTimer = levelData.levelTime;
        score = 0;
        UpdateUI();
        running = true;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    void PrepareMoles()
    {
        foreach (var m in moles) if (m != null) Destroy(m.gameObject);
        moles.Clear();

        int count = Mathf.Clamp(levelData.moleCount, 1, holeTransforms.Length);
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(molePrefab, moleParent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = holeTransforms[i].anchoredPosition;
            MoleUI mole = go.GetComponent<MoleUI>();

            Sprite initialSprite = null;
            int initialIndex = -1;
            if (levelData.contentsSprites != null && levelData.contentsSprites.Length > 0)
            {
                initialIndex = i % levelData.contentsSprites.Length;
                initialSprite = levelData.contentsSprites[initialIndex];
            }

            mole.Setup(initialSprite, initialIndex, levelData.riseDistance, levelData.riseDuration);
            mole.OnHit += OnMoleHit;
            mole.OnHidden += OnMoleHidden;
            moles.Add(mole);
        }
    }

    IEnumerator SpawnLoop()
    {
        if (levelData.spawnMode == SpawnMode.Sequential)
        {
            int idx = 0;
            while (running && levelTimer > 0f)
            {
                var mole = moles[idx % moles.Count];
                if (levelData.contentsSprites != null && levelData.contentsSprites.Length > 0)
                {
                    int spriteIdx = idx % levelData.contentsSprites.Length;
                    mole.Setup(levelData.contentsSprites[spriteIdx], spriteIdx, levelData.riseDistance, levelData.riseDuration);
                }
                mole.PopUp(levelData.visibleTime);

                float wait = levelData.visibleTime + 0.15f;
                yield return new WaitForSeconds(wait);
                idx++;
                levelTimer -= wait;
                UpdateUI();
                yield return null;
            }
        }
        else
        {
            while (running && levelTimer > 0f)
            {
                int concurrent = 0;
                foreach (var m in moles) if (m.IsVisible()) concurrent++;

                if (concurrent < levelData.maxConcurrentVisible)
                {
                    List<MoleUI> hidden = moles.FindAll(x => !x.IsVisible());
                    if (hidden.Count > 0)
                    {
                        MoleUI pick = hidden[Random.Range(0, hidden.Count)];
                        if (levelData.contentsSprites != null && levelData.contentsSprites.Length > 0)
                        {
                            int sIdx = Random.Range(0, levelData.contentsSprites.Length);
                            pick.Setup(levelData.contentsSprites[sIdx], sIdx, levelData.riseDistance, levelData.riseDuration);
                        }
                        pick.PopUp(levelData.visibleTime);
                    }
                }

                float wait = Random.Range(levelData.minSpawnInterval, levelData.maxSpawnInterval);
                yield return new WaitForSeconds(wait);
                levelTimer -= wait;
                UpdateUI();
            }
        }

        EndLevel();
    }

    void OnMoleHit(MoleUI mole)
    {
        bool correct = false;
        if (levelData.correctSpriteIndices != null)
        {
            foreach (var idx in levelData.correctSpriteIndices)
            {
                if (idx == mole.spriteIndex) { correct = true; break; }
            }
        }

        if (correct) score += 10;
        else score -= 2;

        // feedback visual/audio (impleméntarlo aquí)
        UpdateUI();
    }

    void OnMoleHidden(MoleUI mole)
    {
        
        bool wasCorrect = false;
        if (levelData.correctSpriteIndices != null)
        {
            foreach (var idx in levelData.correctSpriteIndices)
                if (idx == mole.spriteIndex) { wasCorrect = true; break; }
        }
        if (wasCorrect) score -= 1;

        // feedback visual/audio (impleméntarlo aquí)
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerText != null) timerText.text = Mathf.CeilToInt(levelTimer).ToString();
        if (scoreText != null) scoreText.text = score.ToString();
    }

    void EndLevel()
    {
        running = false;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        foreach (var m in moles) if (m != null) m.gameObject.SetActive(false);

        Debug.Log("Level ended. Score: " + score);

        OnLevelEnded?.Invoke();
    }
}
