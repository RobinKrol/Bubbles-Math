using UnityEngine;

public class UniversalLanguageSwitch : MonoBehaviour
{
    [Header("Universal Language Switch")]
    [SerializeField] private bool enableKeyboardShortcuts = true;
    [SerializeField] private string englishKey = "E";
    [SerializeField] private string russianKey = "R";
    
    void Update()
    {
        if (!enableKeyboardShortcuts) return;
        
        // Переключение на английский
        if (IsKeyPressed(englishKey))
        {
            SwitchToEnglish();
        }
        
        // Переключение на русский
        if (IsKeyPressed(russianKey))
        {
            SwitchToRussian();
        }
    }
    
    private bool IsKeyPressed(string keyName)
    {
        // Универсальная проверка нажатия клавиши
        return Input.GetKeyDown(keyName) || Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), keyName));
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
    
    // Метод для программного переключения языка
    public void SwitchLanguage(string language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage(language);
            Debug.Log($"Language switched to {language}");
        }
        else
        {
            Debug.LogWarning("LocalizationManager not found!");
        }
    }
    
    // Метод для получения текущего языка
    public string GetCurrentLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            return LocalizationManager.Instance.GetCurrentLanguage();
        }
        return "en";
    }
} 