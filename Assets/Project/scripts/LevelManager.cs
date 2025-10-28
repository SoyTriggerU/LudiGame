using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Holes")]
    [SerializeField] private RectTransform[] backHoleTransforms;
    [SerializeField] private RectTransform[] frontHoleTransforms;

    [Header("Moles")]
    [SerializeField] private GameObject molePrefab;
    [SerializeField] private Transform backMoleParent;
    [SerializeField] private Transform frontMoleParent;

    [Header("UI")]
    [SerializeField] private LevelTimer timer;
    [SerializeField] private QuestionUI questionUI;
    [SerializeField] private LevelCompleteUI completeUI;
    [SerializeField] private OutOfTimeUI outOfTimeUI;

    [SerializeField] private bool acceptInput = true;

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

            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            spawnRoutine = StartCoroutine(SpawnLoop());
        };

        questionUI.ShowQuestion(levelData.questions[currentQuestionIndex]);
    }

    void PrepareMoles()
    {
        foreach (var m in moles)
            if (m != null) Destroy(m.gameObject);
        moles.Clear();

        foreach (var hole in backHoleTransforms)
        {
            Transform maskTransform = hole.Find("mask");

            Transform moleArea = maskTransform.Find("MoleArea");

            GameObject go = Instantiate(molePrefab, moleArea);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Vector2.zero;

            MoleUI mole = go.GetComponent<MoleUI>();
            mole.Setup(null, -1, levelData.riseDistance, levelData.riseDuration);
            mole.OnHit += OnMoleHit;
            moles.Add(mole);
        }

        foreach (var hole in frontHoleTransforms)
        {
            Transform maskTransform = hole.Find("mask");

            Transform moleArea = maskTransform.Find("MoleArea");

            GameObject go = Instantiate(molePrefab, moleArea);
            RectTransform rt = go.GetComponent<RectTransform>();

            rt.anchoredPosition = Vector2.zero;

            MoleUI mole = go.GetComponent<MoleUI>();
            mole.Setup(null, -1, levelData.riseDistance, levelData.riseDuration);
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
            Dictionary<int, MoleUI> spriteToMole = new Dictionary<int, MoleUI>();
            int maxPairs = Mathf.Min(levelData.contentsSprites.Length, moles.Count);
            for (int i = 0; i < maxPairs; i++)
                spriteToMole[i] = moles[i];

            List<int> spriteQueue = new List<int>(Enumerable.Range(0, maxPairs));
            ShuffleList(spriteQueue);

            while (running)
            {
                int concurrent = 0;
                foreach (var m in moles) if (m.IsVisible()) concurrent++;

                if (concurrent < levelData.maxConcurrentVisible)
                {
                    if (spriteQueue.Count == 0)
                    {
                        yield return new WaitUntil(() => {
                            foreach (var mm in moles) if (mm.IsVisible()) return false;
                            return true;
                        });

                        spriteQueue = new List<int>(Enumerable.Range(0, maxPairs));
                        ShuffleList(spriteQueue);
                    }

                    int chosenIndexInQueue = -1;
                    for (int i = 0; i < spriteQueue.Count; i++)
                    {
                        int s = spriteQueue[i];
                        if (spriteToMole.TryGetValue(s, out var candidateMole))
                        {
                            if (!candidateMole.IsVisible())
                            {
                                chosenIndexInQueue = i;
                                break;
                            }
                        }
                    }

                    if (chosenIndexInQueue == -1)
                    {
                        yield return null;
                        continue;
                    }

                    int spriteIdx = spriteQueue[chosenIndexInQueue];
                    spriteQueue.RemoveAt(chosenIndexInQueue);

                    if (spriteToMole.TryGetValue(spriteIdx, out MoleUI pick))
                    {
                        if (levelData.contentsSprites != null && levelData.contentsSprites.Length > spriteIdx)
                        {
                            pick.Setup(levelData.contentsSprites[spriteIdx], spriteIdx, levelData.riseDistance, levelData.riseDuration);
                        }

                        if (pick.IsVisible() || pick.IsAnimating)
                        {
                            pick.ForceHide();
                            yield return new WaitForSeconds(0.05f);
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

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    void ForceHideAllMoles()
    {
        foreach (var m in moles)
        {
            if (m != null)
                m.ForceHide();
        }
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
            ForceHideAllMoles();
            SetMolesActive(false);

            if (timer != null)
                timer.PauseTimer();

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
            if (timer != null)
                timer.ResumeTimer();
        }
        else
        {
            if (timer != null)
                timer.PauseTimer();
            running = false;

            StartCoroutine(ShowLevelCompleteAndEnd());
        }
    }

    private IEnumerator ShowLevelCompleteAndEnd()
    {
        if (completeUI != null)
            yield return completeUI.ShowMessage();

        OnLevelEnded?.Invoke();
        EndLevel();
    }

    private void HandleOutOfTime()
    {
        if (timer != null)
            timer.PauseTimer();
        running = false;

        if (outOfTimeUI != null)
            outOfTimeUI.Show();
    }

    public void RestartCurrentLevel(LevelData data)
    {
        ScoreManager.Instance.SetTempScore(startingScore);

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        running = false;

        foreach (var m in moles)
        {
            if (m != null)
            {
                m.ForceHide();
                m.gameObject.SetActive(false);
            }
        }

        if (timer != null)
        {
            timer.StopTimer();
            timer.ResetTimer(data.levelTime);
            timer.StartTimer(data.levelTime);
        }

        acceptInput = false;

        StartLevel(data);
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