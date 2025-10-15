using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour
{
    public RectTransform[] holeTransforms;
    public GameObject molePrefab;
    public Transform moleParent;
    public LevelTimer timer;
    public QuestionUI questionUI;
    public LevelCompleteUI completeUI;
    public OutOfTimeUI outOfTimeUI;

    public bool acceptInput = true;

    List<MoleUI> moles = new List<MoleUI>();

    private LevelData levelData;
    private int currentQuestionIndex = 0;
    private int startingScore;
    bool running = false;
    private Coroutine spawnRoutine;

    public System.Action OnLevelEnded;


    public void StartLevel(LevelData data)
    {
        levelData = data;
        PrepareMoles();
        currentQuestionIndex = 0;
        timer.ResumeTimer();
        running = true;

        startingScore = ScoreManager.Instance.TempScore;

        ShowCurrentQuestion();

        if (timer != null)
        {
            StartCoroutine(StartTimerWithDelay(0.75f));
        }

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());

        if (outOfTimeUI != null)
            outOfTimeUI.Setup(this);
    }

    void SetMolesActive(bool active)
    {
        foreach (var mole in moles)
        {
            if (mole != null)
                mole.gameObject.SetActive(active);
        }
    }

    void ShowCurrentQuestion()
    {
        if (questionUI == null || levelData.questions.Length == 0) return;

        questionUI.OnSlideInComplete = () =>
        {
            acceptInput = true;
            SetMolesActive(true);
        };

        questionUI.ShowQuestion(levelData.questions[currentQuestionIndex]);
    }
    void PrepareMoles()
    {
        foreach (var m in moles)
            if (m != null) Destroy(m.gameObject);
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
            moles.Add(mole);
        }
    }

    IEnumerator SpawnLoop()
    {
        if (levelData.spawnMode == SpawnMode.Sequential)
        {
            int idx = 0;
            while (running)
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
                yield return null;
            }
        }
        else
        {
            while (running)
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
            }
        }

        EndLevel();
    }

    private IEnumerator StartTimerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (timer != null && levelData != null)
        {
            timer.StartTimer(levelData.levelTime);
            timer.OnTimerEnded += HandleOutOfTime;
        }
    }

    void PauseSpawnLoop()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    void ResumeSpawnLoop()
    {
        if (spawnRoutine == null && running)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    void OnMoleHit(MoleUI mole)
    {
        if (!acceptInput) return;

        int correctIdx = -1;
        if (levelData.correctSpriteIndexPerQuestion != null && currentQuestionIndex < levelData.correctSpriteIndexPerQuestion.Length)
            correctIdx = levelData.correctSpriteIndexPerQuestion[currentQuestionIndex];

        bool correct = (mole.spriteIndex == correctIdx);

        if (correct)
        {
            acceptInput = false;
            PauseSpawnLoop();
            SetMolesActive(false);
            ScoreManager.Instance.AddTempScore(10);
            questionUI.ShowCorrect();

            StartCoroutine(NextQuestionAfterDelay());
        }
        else
        {
            questionUI.ShowWrong();
            ScoreManager.Instance.SubtractTempScore(2);
        }
    }

    IEnumerator NextQuestionAfterDelay()
    {
        float delay = 2.5f;
        if (questionUI != null)
            delay = questionUI.DelayBetweenQuestions;

        yield return new WaitForSeconds(delay);

        currentQuestionIndex++;

        if (currentQuestionIndex < levelData.questions.Length)
        {
            questionUI.ShowQuestion(levelData.questions[currentQuestionIndex]);
            yield return new WaitUntil(() => questionUI.IsSlideInComplete);
            SetMolesActive(true);
            ResumeSpawnLoop();
            acceptInput = true;
        }
        else
        {
            running = false;
            if (timer != null)
                timer.PauseTimer();

            StartCoroutine(ShowLevelCompleteAndEnd());
        }
    }

    private IEnumerator ShowLevelCompleteAndEnd()
    {
        if (completeUI != null)
            yield return completeUI.ShowMessage("NIVELL COMPLETAT!");

        OnLevelEnded?.Invoke();
        EndLevel();
    }

    private void HandleOutOfTime()
    {
        running = false;
        if (timer != null)
            timer.PauseTimer();

        if (outOfTimeUI != null)
            outOfTimeUI.Show();
    }

    public void RestartCurrentLevel(LevelData data)
    {
        ScoreManager.Instance.SetTempScore(startingScore);

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        foreach (var m in moles)
            if (m != null) m.gameObject.SetActive(false);

        if (timer != null)
        {
            timer.StopTimer();
            timer.StartTimer(data.levelTime);
        }

        currentQuestionIndex = 0;
        running = true;

        ShowCurrentQuestion();

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    void EndLevel()
    {
        running = false;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        foreach (var m in moles) if (m != null) m.gameObject.SetActive(false);

        if (timer != null)
            timer.OnTimerEnded -= HandleOutOfTime;
    }
}
