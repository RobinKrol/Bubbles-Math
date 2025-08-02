using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;
    public Image buttonImage;

    private bool isMuted = false; 

    void Start()
    {
        // Инициализация состояния
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        UpdateIcon();
        ApplyMute();
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        UpdateIcon();
        ApplyMute();
    }
    private void UpdateIcon()
    {
        if (buttonImage != null)
            buttonImage.sprite = isMuted ? soundOffIcon : soundOnIcon;
    }
    private void ApplyMute()
    {
        AudioListener.volume = isMuted ? 0f : 1f;
    }
}
