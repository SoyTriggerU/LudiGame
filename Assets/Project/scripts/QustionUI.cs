using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class QuestionUI : MonoBehaviour
{
    public TMP_Text questionText;
    public Image bubbleImage;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    public float slideSpeed = 300f;
    public float fadeDuration = 0.5f;
    public float DelayBetweenQuestions = 2.5f;
    public float offscreenY = 800f;   
    public float topVisibleY = 250f;

    public Color normalColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private Coroutine moveRoutine;

    public System.Action OnSlideInComplete;

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
        bubbleImage.color = normalColor;
        questionText.text = "";
    }

    public void ShowQuestion(string question)
    {
        questionText.text = question;
        bubbleImage.color = normalColor;
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SlideIn());
    }

    public void ShowCorrect()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        bubbleImage.color = correctColor;
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
        rect.anchoredPosition = new Vector2(0, offscreenY);

        while (rect.anchoredPosition.y > topVisibleY)
        {
            rect.anchoredPosition -= new Vector2(0, slideSpeed * Time.deltaTime);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, topVisibleY);

        OnSlideInComplete?.Invoke();
    }

    IEnumerator FlashWrong()
    {
        bubbleImage.color = wrongColor;
        yield return new WaitForSeconds(0.5f);
        bubbleImage.color = normalColor;
    }

    IEnumerator FlashAndSlideOut(Color targetColor)
    {
        bubbleImage.color = targetColor;
        yield return new WaitForSeconds(0.5f);

        while (rect.anchoredPosition.y < offscreenY)
        {
            rect.anchoredPosition += new Vector2(0, slideSpeed * Time.deltaTime);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, offscreenY);
    }
}
