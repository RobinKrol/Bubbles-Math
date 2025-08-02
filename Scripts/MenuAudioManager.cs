using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{   public static MenuAudioManager Instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip[] clickSounds;
    public AudioClip menuMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayClick()
     {
         if (sfxSource && clickSounds != null && clickSounds.Length > 0)
         {
             int idx = Random.Range(0, clickSounds.Length);
             sfxSource.PlayOneShot(clickSounds[idx]);
         }
     }
    void OnApplicationFocus(bool hasFocus)
    {
        AudioListener.pause = !hasFocus;
        
    }
    void OnApplicationPause(bool pauseStatus)
    {
        AudioListener.pause = pauseStatus;
    }
}
