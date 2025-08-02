using UnityEngine;
using UnityEngine.InputSystem;

public class QuickLanguageSwitch : MonoBehaviour
{
    [Header("Quick Language Switch")]
    [SerializeField] private bool enableKeyboardShortcuts = true;
    [SerializeField] private Key englishKey = Key.E;
    [SerializeField] private Key russianKey = Key.R;
    
    private Keyboard keyboard;
    
    void Start()
    {
        // Получаем ссылку на клавиатуру
        keyboard = Keyboard.current;
    }
    
    void Update()
    {
        if (!enableKeyboardShortcuts || keyboard == null) return;
        
        // Переключение на английский по нажатию E
        if (keyboard[englishKey].wasPressedThisFrame)
        {
            SwitchToEnglish();
        }
        
        // Переключение на русский по нажатию R
        if (keyboard[russianKey].wasPressedThisFrame)
        {
            SwitchToRussian();
        }
    }
    
    // Публичные методы для вызова из других скриптов или UI
    [ContextMenu("Switch to English")]
    public void SwitchToEnglish()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage("en");
            Debug.Log("Language switched to English");
        }
        else
        {
            Debug.LogWarning("LocalizationManager not found!");
        }
    }
    
    [ContextMenu("Switch to Russian")]
    public void SwitchToRussian()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage("ru");
            Debug.Log("Language switched to Russian");
        }
        else
        {
            Debug.LogWarning("LocalizationManager not found!");
        }
    }
    
    // Метод для переключения на противоположный язык
    [ContextMenu("Toggle Language")]
    public void ToggleLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            string currentLang = LocalizationManager.Instance.GetCurrentLanguage();
            string newLang = currentLang == "en" ? "ru" : "en";
            LocalizationManager.Instance.SwitchLanguage(newLang);
            Debug.Log($"Language toggled from {currentLang} to {newLang}");
        }
        else
        {
            Debug.LogWarning("LocalizationManager not found!");
        }
    }
    
    // Метод для получения информации о текущем языке
    [ContextMenu("Show Current Language")]
    public void ShowCurrentLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            string currentLang = LocalizationManager.Instance.GetCurrentLanguage();
            Debug.Log($"Current language: {currentLang}");
        }
        else
        {
            Debug.LogWarning("LocalizationManager not found!");
        }
    }
} 