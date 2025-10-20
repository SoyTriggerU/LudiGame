using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class OutOfTimeUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    public Button restartButton;
    public Button menuButton;
    public LevelSelector levelSelector;
    private LevelManager levelManager;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Setup(LevelManager manager)
    {
        levelManager = manager;
        restartButton.onClick.AddListener(RestartLevel);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        messageText.text = "S'HA ACABAT EL TEMPS!";
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    void RestartLevel()
    {
        if (levelSelector != null)
        {
            levelSelector.RestartCurrentLevel();
            gameObject.SetActive(false);
        }
    }
}
