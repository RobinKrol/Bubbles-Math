using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент для тестирования локализации в игре
/// Добавляет кнопки для быстрого переключения языков
/// </summary>
public class LanguageTester : MonoBehaviour
{
    [Header("Language Test UI")]
    [Tooltip("Панель с кнопками переключения языков")]
    public GameObject languageTestPanel;
    
    [Tooltip("Кнопка переключения на русский")]
    public Button russianButton;
    
    [Tooltip("Кнопка переключения на английский")]
    public Button englishButton;
    
    [Tooltip("Кнопка показа/скрытия панели тестирования")]
    public Button togglePanelButton;
    
    [Header("Settings")]
    [Tooltip("Показывать панель тестирования при старте")]
    public bool showOnStart = false;
    
    [Tooltip("Автоматически обновлять все панели при смене языка")]
    public bool autoUpdatePanels = true;

    void Start()
    {
        SetupButtons();
        
        if (showOnStart)
        {
            ShowTestPanel();
        }
        else
        {
            HideTestPanel();
        }
    }
    
    private void SetupButtons()
    {
        // Настройка кнопки русского языка
        if (russianButton != null)
        {
            russianButton.onClick.AddListener(() => SwitchLanguage("ru"));
        }
        
        // Настройка кнопки английского языка
        if (englishButton != null)
        {
            englishButton.onClick.AddListener(() => SwitchLanguage("en"));
        }
        
        // Настройка кнопки показа/скрытия панели
        if (togglePanelButton != null)
        {
            togglePanelButton.onClick.AddListener(ToggleTestPanel);
        }
    }
    
    /// <summary>
    /// Переключает язык
    /// </summary>
    public void SwitchLanguage(string language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage(language);
            
            if (autoUpdatePanels)
            {
                UpdateAllPanels();
            }
            
            Debug.Log($"Язык переключен на: {language}");
        }
        else
        {
            Debug.LogWarning("LocalizationManager не найден!");
        }
    }
    
    /// <summary>
    /// Переключает на русский язык
    /// </summary>
    public void SwitchToRussian()
    {
        SwitchLanguage("ru");
    }
    
    /// <summary>
    /// Переключает на английский язык
    /// </summary>
    public void SwitchToEnglish()
    {
        SwitchLanguage("en");
    }
    
    /// <summary>
    /// Показывает панель тестирования
    /// </summary>
    public void ShowTestPanel()
    {
        if (languageTestPanel != null)
        {
            languageTestPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Скрывает панель тестирования
    /// </summary>
    public void HideTestPanel()
    {
        if (languageTestPanel != null)
        {
            languageTestPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Переключает видимость панели тестирования
    /// </summary>
    public void ToggleTestPanel()
    {
        if (languageTestPanel != null)
        {
            languageTestPanel.SetActive(!languageTestPanel.activeSelf);
        }
    }
    
    /// <summary>
    /// Обновляет все панели в сцене
    /// </summary>
    private void UpdateAllPanels()
    {
        // Обновляем MenuPanelManager
        MenuPanelManager menuManager = FindFirstObjectByType<MenuPanelManager>();
        if (menuManager != null)
        {
            menuManager.UpdateAllPanelsLocalization();
        }
        
        // Обновляем GameScenePanelManager
        GameScenePanelManager gameManager = FindFirstObjectByType<GameScenePanelManager>();
        if (gameManager != null)
        {
            gameManager.UpdateAllPanelsLocalization();
        }
        
        // Принудительно обновляем все тексты
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ForceUpdateAllTexts();
        }
    }
    
    /// <summary>
    /// Получает текущий язык
    /// </summary>
    public string GetCurrentLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            return LocalizationManager.Instance.GetCurrentLanguage();
        }
        return "unknown";
    }
    
    /// <summary>
    /// Выводит информацию о текущем состоянии локализации
    /// </summary>
    public void PrintLocalizationInfo()
    {
        string currentLang = GetCurrentLanguage();
        Debug.Log($"Текущий язык: {currentLang}");
        Debug.Log($"LocalizationManager готов: {LocalizationManager.Instance != null}");
        
        // Проверяем панели
        MenuPanelManager menuManager = FindFirstObjectByType<MenuPanelManager>();
        GameScenePanelManager gameManager = FindFirstObjectByType<GameScenePanelManager>();
        
        Debug.Log($"MenuPanelManager найден: {menuManager != null}");
        Debug.Log($"GameScenePanelManager найден: {gameManager != null}");
    }
} 