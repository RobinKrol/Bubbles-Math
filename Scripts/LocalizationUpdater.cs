using UnityEngine;

/// <summary>
/// Компонент для автоматического обновления локализации при активации панелей и UI элементов
/// </summary>
public class LocalizationUpdater : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Обновлять локализацию при активации объекта")]
    public bool updateOnEnable = true;
    
    [Tooltip("Обновлять локализацию при старте")]
    public bool updateOnStart = true;
    
    [Tooltip("Обновлять локализацию при смене языка")]
    public bool updateOnLanguageChange = true;
    
    [Tooltip("Обновлять локализацию во всех дочерних объектах")]
    public bool updateChildren = true;

    void Start()
    {
        if (updateOnStart)
        {
            UpdateLocalization();
        }
        
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
        }
    }
    
    void OnEnable()
    {
        if (updateOnEnable)
        {
            UpdateLocalization();
        }
    }
    
    void OnDestroy()
    {
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void OnLanguageChanged(string newLanguage)
    {
        UpdateLocalization();
    }
    
    /// <summary>
    /// Обновляет локализацию в этом объекте и его дочерних объектах
    /// </summary>
    public void UpdateLocalization()
    {
        if (LocalizationManager.Instance == null) return;
        
        if (updateChildren)
        {
            LocalizationManager.Instance.UpdateTextsInGameObject(gameObject);
        }
        else
        {
            // Обновляем только в этом объекте
            LocalizedTextComponent[] components = GetComponents<LocalizedTextComponent>();
            foreach (var component in components)
            {
                if (component != null)
                {
                    component.UpdateText();
                }
            }
        }
    }
    
    /// <summary>
    /// Принудительное обновление локализации
    /// </summary>
    public void ForceUpdate()
    {
        UpdateLocalization();
    }
} 