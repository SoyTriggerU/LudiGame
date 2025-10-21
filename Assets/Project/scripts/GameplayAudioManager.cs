using System.Collections;
using UnityEngine;

public class BackgroundAudioManager : MonoBehaviour
{
    [Header("Clips del Gameplay")]
    [SerializeField] private AudioSource baseSource;
    [SerializeField] private AudioSource ambientSource1;
    [SerializeField] private AudioSource ambientSource2;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float startDelay = 1f;

    public static BackgroundAudioManager Instance { get; private set; }

    void Start()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

        StartCoroutine(FadeInGameplayMusic());
    }

    private IEnumerator FadeInGameplayMusic()
    {
        if (startDelay > 0.0f)
            yield return new WaitForSeconds(startDelay);

        float baseTarget = baseSource.volume;
        float amb1Target = ambientSource1.volume;
        float amb2Target = ambientSource2.volume;

        baseSource.volume = 0.0f;
        ambientSource1.volume = 0.0f;
        ambientSource2.volume = 0.0f;

        baseSource.Play();
        ambientSource1.Play();
        ambientSource2.Play();

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float factor = t / fadeInDuration;

            baseSource.volume = Mathf.Lerp(0f, baseTarget, factor);
            ambientSource1.volume = Mathf.Lerp(0f, amb1Target, factor);
            ambientSource2.volume = Mathf.Lerp(0f, amb2Target, factor);

            yield return null;
        }

        baseSource.volume = baseTarget;
        ambientSource1.volume = amb1Target;
        ambientSource2.volume = amb2Target;
    }
}
