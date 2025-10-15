using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditsButton;

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
        SceneManager.LoadScene("SubjectMenuScene");
    }

    void OnCreditsClicked()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
