using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

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

    void OnExitClicked()
    {
        ClickSoundManager.Instance.PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
