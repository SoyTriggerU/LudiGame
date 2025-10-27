using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsClicked);
    }

    void OnPlayClicked()
    {
        ClickSoundManager.Instance.PlayClick();
        SceneManager.LoadScene("SubjectMenuScene");
    }

    void OnCreditsClicked()
    {
        ClickSoundManager.Instance.PlayClick();
        SceneManager.LoadScene("CreditsScene");
    }

    void OnSettingsClicked()
    {
        ClickSoundManager.Instance.PlayClick();
        SceneManager.LoadScene("SettingsScene");
    }
}
