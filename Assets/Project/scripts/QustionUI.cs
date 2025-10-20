using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class QuestionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Image bubbleImage;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    public float slideSpeed = 300f;
    public float fadeDuration = 0.5f;
    public float DelayBetweenQuestions = 2.5f;
    public float offscreenY = 800f;   
    public float topVisibleY = 250f;

    [SerializeField] private Sprite normalColor;
    [SerializeField] private Sprite correctColor;
    [SerializeField] private Sprite wrongColor;

    private Coroutine moveRoutine;

    public System.Action OnSlideInComplete;
    public bool IsSlideInComplete { get; private set; } = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        ResetQuestionUI();
    }

    void ResetQuestionUI()
    {
        StopAllCoroutines();
        rect.anchoredPosition = new Vector2(0, offscreenY);
        bubbleImage.sprite = normalColor;
        questionText.text = "";
    }

    public void ShowQuestion(string question)
    {
        questionText.text = question;
        bubbleImage.sprite = normalColor;
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SlideIn());
    }

    public void ShowCorrect()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        bubbleImage.sprite = correctColor;
        moveRoutine = StartCoroutine(SlideOutCorrect());
    }

    IEnumerator SlideOutCorrect()
    { 
        yield return new WaitForSeconds(0.8f);

        while (rect.anchoredPosition.y < offscreenY)
        {
            rect.anchoredPosition += new Vector2(0, slideSpeed * Time.deltaTime);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, offscreenY);
    }

    public void ShowWrong()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        StartCoroutine(FlashWrong());
    }

    IEnumerator SlideIn()
    {
        IsSlideInComplete = false;
        rect.anchoredPosition = new Vector2(0, offscreenY);

        while (rect.anchoredPosition.y > topVisibleY)
        {
            rect.anchoredPosition -= new Vector2(0, slideSpeed * Time.deltaTime);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, topVisibleY);
        IsSlideInComplete = true;
        OnSlideInComplete?.Invoke();
    }

    IEnumerator FlashWrong()
    {
        bubbleImage.sprite = wrongColor;
        yield return new WaitForSeconds(0.5f);
        bubbleImage.sprite = normalColor;
    }
}
