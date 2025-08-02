using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip backgroundMusic;
    public AudioClip[] clickSounds;
    public AudioClip successSound;
    public AudioClip failSound;

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
    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (musicSource && backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
{
    if (musicSource != null)
        musicSource.Stop();
}
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    public void PlayClick()
     {
         if (sfxSource && clickSounds != null && clickSounds.Length > 0)
         {
             int idx = Random.Range(0, clickSounds.Length);
             sfxSource.PlayOneShot(clickSounds[idx]);
         }
     }
    public void PlaySuccess() => PlaySFX(successSound);
    public void PlayFail() => PlaySFX(failSound);
    void OnApplicationFocus(bool hasFocus)
    {
        AudioListener.pause = !hasFocus;
        // НЕ управляем Time.timeScale здесь - это делает GameModeManager
    }
    void OnApplicationPause(bool pauseStatus)
    {
        AudioListener.pause = pauseStatus;
        // НЕ управляем Time.timeScale здесь - это делает GameModeManager
    }
}

