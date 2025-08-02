using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Управление панелями для игровой сцены Bubbles Math
/// </summary>
public class GameScenePanelManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelData
    {
        public string panelName;
        public GameObject panelObject;
        public bool updateLocalizationOnShow = true;
        public bool updateLocalizationOnHide = false;
    }

    [Header("Game Scene Panels")]
    [Tooltip("Панель игры")]
    public GameObject gamePanel;
    
    [Tooltip("Панель паузы")]
    public GameObject pausePanel;
    
    [Tooltip("Панель продолжения игры")]
    public GameObject continueGamePanel;
    
    [Tooltip("Панель таблицы лидеров")]
    public GameObject leaderboardPanel;
    
    [Header("Settings")]
    [Tooltip("Автоматически добавлять LocalizationUpdater к панелям")]
    public bool autoAddLocalizationUpdater = true;
    
    [Tooltip("Автоматически обновлять локализацию при смене языка")]
    public bool autoUpdateOnLanguageChange = true;

    private Dictionary<string, PanelData> panelDictionary = new Dictionary<string, PanelData>();
    private GameObject currentActivePanel = null;

    void Start()
    {
        // Настраиваем панели
        SetupPanels();
        
        // Добавляем LocalizationUpdater к панелям если нужно
        if (autoAddLocalizationUpdater)
        {
            AddLocalizationUpdaters();
        }
        
        // Подписываемся на события локализации
        if (autoUpdateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
        }
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        if (autoUpdateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void OnLanguageChanged(string newLanguage)
    {
        // Обновляем локализацию в активной панели
        if (currentActivePanel != null)
        {
            UpdatePanelLocalization(currentActivePanel);
        }
    }
    
    private void SetupPanels()
    {
        panelDictionary.Clear();
        
        // Добавляем панели в словарь
        if (gamePanel != null)
        {
            panelDictionary["GamePanel"] = new PanelData
            {
                panelName = "GamePanel",
                panelObject = gamePanel,
                updateLocalizationOnShow = true,
                updateLocalizationOnHide = false
            };
        }
        
        if (pausePanel != null)
        {
            panelDictionary["PausePanel"] = new PanelData
            {
                panelName = "PausePanel",
                panelObject = pausePanel,
                updateLocalizationOnShow = true,
                updateLocalizationOnHide = false
            };
        }
        
        if (continueGamePanel != null)
        {
            panelDictionary["ContinueGamePanel"] = new PanelData
            {
                panelName = "ContinueGamePanel",
                panelObject = continueGamePanel,
                updateLocalizationOnShow = true,
                updateLocalizationOnHide = false
            };
        }
        
        if (leaderboardPanel != null)
        {
            panelDictionary["LeaderboardPanel"] = new PanelData
            {
                panelName = "LeaderboardPanel",
                panelObject = leaderboardPanel,
                updateLocalizationOnShow = true,
                updateLocalizationOnHide = false
            };
        }
    }
    
    private void AddLocalizationUpdaters()
    {
        GameObject[] panels = { gamePanel, pausePanel, continueGamePanel, leaderboardPanel };
        
        foreach (var panel in panels)
        {
            if (panel != null && panel.GetComponent<LocalizationUpdater>() == null)
            {
                var updater = panel.AddComponent<LocalizationUpdater>();
                updater.updateOnEnable = true;
                updater.updateOnStart = true;
                updater.updateOnLanguageChange = true;
                updater.updateChildren = true;
                
                Debug.Log($"Добавлен LocalizationUpdater к панели: {panel.name}");
            }
        }
    }
    
    /// <summary>
    /// Показывает панель по имени
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (panelDictionary.TryGetValue(panelName, out PanelData panelData))
        {
            // Скрываем текущую активную панель
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
            }
            
            // Показываем новую панель
            panelData.panelObject.SetActive(true);
            currentActivePanel = panelData.panelObject;
            
            // Обновляем локализацию если нужно
            if (panelData.updateLocalizationOnShow)
            {
                UpdatePanelLocalization(panelData.panelObject);
            }
        }
        else
        {
            Debug.LogWarning($"Panel '{panelName}' not found!");
        }
    }
    
    /// <summary>
    /// Скрывает панель по имени
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (panelDictionary.TryGetValue(panelName, out PanelData panelData))
        {
            panelData.panelObject.SetActive(false);
            
            // Обновляем локализацию если нужно
            if (panelData.updateLocalizationOnHide)
            {
                UpdatePanelLocalization(panelData.panelObject);
            }
            
            if (currentActivePanel == panelData.panelObject)
            {
                currentActivePanel = null;
            }
        }
    }
    
    /// <summary>
    /// Обновляет локализацию в панели
    /// </summary>
    private void UpdatePanelLocalization(GameObject panel)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.UpdateTextsInGameObject(panel);
        }
    }
    
    // Методы для управления панелями (удобные обертки)
    
    /// <summary>
    /// Показать игровую панель
    /// </summary>
    public void ShowGame()
    {
        ShowPanel("GamePanel");
    }
    
    /// <summary>
    /// Показать паузу
    /// </summary>
    public void ShowPause()
    {
        ShowPanel("PausePanel");
    }
    
    /// <summary>
    /// Показать панель продолжения игры
    /// </summary>
    public void ShowContinueGame()
    {
        ShowPanel("ContinueGamePanel");
    }
    
    /// <summary>
    /// Показать таблицу лидеров
    /// </summary>
    public void ShowLeaderboard()
    {
        ShowPanel("LeaderboardPanel");
    }
    
    /// <summary>
    /// Скрыть все панели
    /// </summary>
    public void HideAllPanels()
    {
        foreach (var panel in panelDictionary.Values)
        {
            if (panel.panelObject != null)
            {
                panel.panelObject.SetActive(false);
            }
        }
        currentActivePanel = null;
    }
    
    /// <summary>
    /// Принудительно обновить локализацию во всех панелях
    /// </summary>
    public void UpdateAllPanelsLocalization()
    {
        foreach (var panel in panelDictionary.Values)
        {
            if (panel.panelObject != null && panel.panelObject.activeInHierarchy)
            {
                UpdatePanelLocalization(panel.panelObject);
            }
        }
    }
    
    /// <summary>
    /// Получить текущую активную панель
    /// </summary>
    public GameObject GetCurrentActivePanel()
    {
        return currentActivePanel;
    }
    
    /// <summary>
    /// Проверяет, активна ли панель
    /// </summary>
    public bool IsPanelActive(string panelName)
    {
        if (panelDictionary.TryGetValue(panelName, out PanelData panelData))
        {
            return panelData.panelObject.activeInHierarchy;
        }
        return false;
    }
} 