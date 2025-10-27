using UnityEngine;
using UnityEngine.UI;

public class SoundSliderBinder : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        if (SoundSettingsManager.Instance == null) return;

        musicSlider.value = SoundSettingsManager.Instance.musicVolume;
        sfxSlider.value = SoundSettingsManager.Instance.sfxVolume;

        musicSlider.onValueChanged.AddListener(SoundSettingsManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundSettingsManager.Instance.SetSFXVolume);
    }
}
