using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            soundData = new SoundData();
            soundData.Load();
            ApplySoundSettings();

            PlayBGM(SoundType.BGMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Audio Sources")]
    [SerializeField] public AudioSource _bgmSource; // Nhạc nền
    [SerializeField] public AudioSource _sfxSource; // Hiệu ứng

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] _soundList;

    public SoundData soundData;

    /// <summary>
    /// Phát nhạc nền, tự động loop.
    /// </summary>
    public static void PlayBGM(SoundType sound, float volume = 0.5f)
    {
        Instance._bgmSource.clip = Instance._soundList[(int)sound];
        Instance._bgmSource.volume = volume;
        Instance._bgmSource.loop = true;
        Instance._bgmSource.Play();
    }

    /// <summary>
    /// Dừng nhạc nền.
    /// </summary>
    public static void StopBGM()
    {
        Instance._bgmSource.Stop();
    }

    /// <summary>
    /// Phát âm thanh hiệu ứng.
    /// </summary>
    public static void PlaySFX(SoundType sound, float volume = 1)
    {
        Instance._sfxSource.pitch = Random.Range(0.9f, 1.1f);

        Instance._sfxSource.PlayOneShot(Instance._soundList[(int)sound], volume);
    }

    public void ApplySoundSettings()
    {
        // Áp dụng trạng thái mute
        _bgmSource.mute = soundData.isMusicMute;
        _sfxSource.mute = soundData.isSoundMute;
    }

    public void ToggleSound()
    {
        soundData.isSoundMute = !soundData.isSoundMute;
        _sfxSource.mute = soundData.isSoundMute;
        soundData.Save();
    }

    public void ToggleMusic()
    {
        soundData.isMusicMute = !soundData.isMusicMute;
        _bgmSource.mute = soundData.isMusicMute;
        soundData.Save();
    }
}
public enum SoundType
{
    BGMusic,
    IGMusic,
    Click
}

[System.Serializable]
public class SoundData
{
    public bool isSoundMute;
    public bool isMusicMute;

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("SoundData", json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("SoundData"))
        {
            string json = PlayerPrefs.GetString("SoundData");
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            isSoundMute = false;
            isMusicMute = false;
            Save();
        }
    }
}
