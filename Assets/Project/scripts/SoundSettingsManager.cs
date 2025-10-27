using UnityEngine;

public class SoundSettingsManager : MonoBehaviour
{
    public static SoundSettingsManager Instance { get; private set; }

    [Header("Música")]
    [SerializeField] private AudioSource[] musicSources;

    [Header("Efectos (SFX)")]
    [SerializeField] private AudioSource[] sfxSources;
    
    public float musicVolume = 0.5f;
    public float sfxVolume = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (MusicManager.Instance != null && MusicManager.Instance.GetAudioSource() != null)
        {
            RegisterMusicSource(MusicManager.Instance.GetAudioSource());
        }

        ApplyVolumes();
    }

    public void RegisterMusicSource(AudioSource source)
    {
        if (source != null && !System.Array.Exists(musicSources, s => s == source))
        {
            var list = new System.Collections.Generic.List<AudioSource>(musicSources);
            list.Add(source);
            musicSources = list.ToArray();
            source.volume = musicVolume;
        }
    }

    public void RegisterSFXSource(AudioSource source)
    {
        if (source != null && !System.Array.Exists(sfxSources, s => s == source))
        {
            var list = new System.Collections.Generic.List<AudioSource>(sfxSources);
            list.Add(source);
            sfxSources = list.ToArray();
            source.volume = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        foreach (var m in musicSources)
            if (m != null) m.volume = musicVolume;

        foreach (var s in sfxSources)
            if (s != null) s.volume = sfxVolume;
    }

    public bool IsRegistered(AudioSource source)
    {
        if (source == null) return false;
        foreach (var s in musicSources)
            if (s == source) return true;
        foreach (var s in sfxSources)
            if (s == source) return true;
        return false;
    }

}
