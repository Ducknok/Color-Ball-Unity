using UnityEngine;
using UnityEngine.Audio; // Bắt buộc phải có để dùng AudioMixer

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [Tooltip("Kéo MainMixer vào đây")]
    public AudioMixer mainMixer;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("Game SFX Clips")]
    public AudioClip coinClip;
    public AudioClip scoreClip;
    public AudioClip obstacleHitClip;
    public AudioClip gameOverClip;

    [Header("UI SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip scoreTickClip;
    public AudioClip scoreFinishClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 0 && sfxSource == null) sfxSource = sources[0];
            if (sources.Length > 1 && musicSource == null) musicSource = sources[1];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // LỖI SỐ 1 ĐÃ FIX: Dùng Coroutine để đợi Mixer thức dậy
    private void Start()
    {
        StartCoroutine(InitAudioRoutine());
    }

    private System.Collections.IEnumerator InitAudioRoutine()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Đợi 0.1s
        LoadVolumeSettings();
        PlayMusic(backgroundMusic);
    }

    private void LoadVolumeSettings()
    {
        // LỖI SỐ 3 ĐÃ FIX: Đổi "Music" thành "MusicVolume" cho khớp với UI
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);
    }

    public void SetMusicVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            // LỖI SỐ 2 ĐÃ FIX: Thêm Mathf.Clamp để tránh lỗi -Infinity
            float dbVolume = Mathf.Clamp(Mathf.Log10(sliderValue) * 40f, -80f, 0f);
            mainMixer.SetFloat("MusicVol", dbVolume);
        }

        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            float dbVolume = Mathf.Clamp(Mathf.Log10(sliderValue) * 40f, -80f, 0f);
            mainMixer.SetFloat("SFXVol", dbVolume);
        }

        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.volume = 1f;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    private void PlaySFX(AudioClip clip, float clipBaseVolume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, clipBaseVolume);
        }
    }

    public void PlayCoin() => PlaySFX(coinClip);
    public void PlayScore() => PlaySFX(scoreClip, 1.2f);
    public void PlayHit() => PlaySFX(obstacleHitClip);
    public void PlayGameOver() => PlaySFX(gameOverClip);
    public void PlayClick() => PlaySFX(buttonClickClip, 0.8f);

    public void PlayScoreTick()
    {
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        PlaySFX(scoreTickClip, 0.5f);
        sfxSource.pitch = 1f;
    }

    public void PlayScoreFinish() => PlaySFX(scoreFinishClip);
}