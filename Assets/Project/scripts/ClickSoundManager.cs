using UnityEngine;

public class ClickSoundManager : MonoBehaviour
{
    public static ClickSoundManager Instance;

    [SerializeField] private AudioSource clickSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (clickSound != null && clickSound.clip != null)
        {
            if (SoundSettingsManager.Instance != null)
                SoundSettingsManager.Instance.RegisterSFXSource(clickSound);

            clickSound.PlayOneShot(clickSound.clip);
        }
    }
}
