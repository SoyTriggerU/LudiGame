using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubjectMenu : MonoBehaviour
{
    public LevelSelector levelSelector;
    public GameObject menuPanel;

    public void SelectMath() => SelectSubject(LevelSelector.Subject.Math);
    public void SelectArt() => SelectSubject(LevelSelector.Subject.Art);
    public void SelectCatalan() => SelectSubject(LevelSelector.Subject.Catalan);
    public void SelectSpanish() => SelectSubject(LevelSelector.Subject.Spanish);

    private void SelectSubject(LevelSelector.Subject subject)
    {
        GameManager.Instance.selectedSubject = subject;

        SceneManager.LoadScene("GameScene");
    }
}
