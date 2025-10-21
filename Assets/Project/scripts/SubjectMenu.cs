using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubjectMenu : MonoBehaviour
{
    [SerializeField] private LevelSelector levelSelector;
    [SerializeField] private GameObject menuPanel;

    public void SelectMath() => SelectSubject(LevelSelector.Subject.Math);
    public void SelectArt() => SelectSubject(LevelSelector.Subject.English);
    public void SelectCatalan() => SelectSubject(LevelSelector.Subject.Catalan);
    public void SelectSpanish() => SelectSubject(LevelSelector.Subject.Spanish);

    private void SelectSubject(LevelSelector.Subject subject)
    {
        ClickSoundManager.Instance.PlayClick();
        GameManager.Instance.selectedSubject = subject;
        SceneManager.LoadScene("GameScene");
    }

    public void GoBackToMainMenu()
    {
        ClickSoundManager.Instance.PlayClick();
        SceneManager.LoadScene("MainScreenScene");
    }

    public void CreditScene()
    {
        ClickSoundManager.Instance.PlayClick();
        SceneManager.LoadScene("CreditsScene");
    }
}
