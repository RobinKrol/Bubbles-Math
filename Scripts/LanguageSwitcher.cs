using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageSwitcher : MonoBehaviour
{
    [Header("Language Settings")]
    [SerializeField] private Button languageButton;
    [SerializeField] private TextMeshProUGUI languageButtonText;
    [SerializeField] private string[] availableLanguages = { "en", "ru" };
    [SerializeField] private string[] languageNames = { "English", "Русский" };
    
    private int currentLanguageIndex = 0;
    
    void Start()
    {
        // Находим кнопку если не назначена
        if (languageButton == null)
        {
            languageButton = GetComponent<Button>();
        }
        
        // Находим текст кнопки если не назначен
        if (languageButtonText == null)
        {
            languageButtonText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Подписываемся на событие смены языка
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
        }
        
        // Устанавливаем начальный язык
        SetInitialLanguage();
        
        // Подключаем обработчик нажатия
        if (languageButton != null)
        {
            languageButton.onClick.AddListener(SwitchLanguage);
        }
    }
    
    void OnDestroy()
    {
        // Отписываемся от события
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void SetInitialLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            string currentLang = LocalizationManager.Instance.GetCurrentLanguage();
            
            // Находим индекс текущего языка
            for (int i = 0; i < availableLanguages.Length; i++)
            {
                if (availableLanguages[i] == currentLang)
                {
                    currentLanguageIndex = i;
                    break;
                }
            }
            
            UpdateButtonText();
        }
    }
    
    private void OnLanguageChanged(string newLanguage)
    {
        // Обновляем индекс при смене языка
        for (int i = 0; i < availableLanguages.Length; i++)
        {
            if (availableLanguages[i] == newLanguage)
            {
                currentLanguageIndex = i;
                break;
            }
        }
        
        UpdateButtonText();
    }
    
    public void SwitchLanguage()
    {
        if (LocalizationManager.Instance == null) return;
        
        // Переключаем на следующий язык
        currentLanguageIndex = (currentLanguageIndex + 1) % availableLanguages.Length;
        string newLanguage = availableLanguages[currentLanguageIndex];
        
        Debug.Log($"Switching language to: {newLanguage}");
        
        // Вызываем переключение языка
        LocalizationManager.Instance.SwitchLanguage(newLanguage);
    }
    
    private void UpdateButtonText()
    {
        if (languageButtonText != null && currentLanguageIndex < languageNames.Length)
        {
            languageButtonText.text = languageNames[currentLanguageIndex];
        }
    }
    
    // Публичные методы для программного переключения
    public void SwitchToEnglish()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage("en");
        }
    }
    
    public void SwitchToRussian()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage("ru");
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
    
    // Метод для проверки, является ли текущий язык английским
    public bool IsEnglish()
    {
        return GetCurrentLanguage() == "en";
    }
    
    // Метод для проверки, является ли текущий язык русским
    public bool IsRussian()
    {
        return GetCurrentLanguage() == "ru";
    }
} 