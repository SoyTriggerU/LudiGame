using UnityEngine;
using TMPro;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private AudioSource LevelPassedAudioSource;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public IEnumerator ShowMessage(string message)
    {
        LevelPassedAudioSource.Play();
        messageText.text = message;

       

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(holdTime);

        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
