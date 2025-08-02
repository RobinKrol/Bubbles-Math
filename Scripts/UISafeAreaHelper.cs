using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Вспомогательный скрипт для автоматической настройки UI элементов
/// в безопасной зоне для WebGL платформы
/// </summary>
public class UISafeAreaHelper : MonoBehaviour
{
    [Header("Safe Area Settings")]
    [SerializeField] private bool applySafeArea = true;
    [SerializeField] private float marginPercent = 0.05f; // 5% отступ
    
    [Header("Element Type")]
    [SerializeField] private ElementType elementType = ElementType.Auto;
    
    public enum ElementType
    {
        Auto,
        Button,
        Panel,
        Text,
        Image,
        Custom
    }
    
    private RectTransform rectTransform;
    private Vector2 originalAnchors;
    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;
    private bool isInitialized = false;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"UISafeAreaHelper: RectTransform не найден на {gameObject.name}");
            return;
        }
        
        // Сохраняем оригинальные настройки
        originalAnchors = new Vector2(rectTransform.anchorMin.x, rectTransform.anchorMin.y);
        originalOffsetMin = rectTransform.offsetMin;
        originalOffsetMax = rectTransform.offsetMax;
        
        isInitialized = true;
    }
    
    void Start()
    {
        if (isInitialized && applySafeArea)
        {
            ApplySafeArea();
        }
    }
    
    /// <summary>
    /// Применяет безопасную зону к UI элементу
    /// </summary>
    public void ApplySafeArea()
    {
        if (!isInitialized || rectTransform == null) return;
        
        // Определяем тип элемента если установлен Auto
        if (elementType == ElementType.Auto)
        {
            DetermineElementType();
        }
        
        // Применяем настройки в зависимости от типа элемента
        switch (elementType)
        {
            case ElementType.Button:
                ApplyButtonSafeArea();
                break;
            case ElementType.Panel:
                ApplyPanelSafeArea();
                break;
            case ElementType.Text:
                ApplyTextSafeArea();
                break;
            case ElementType.Image:
                ApplyImageSafeArea();
                break;
            case ElementType.Custom:
                ApplyCustomSafeArea();
                break;
        }
        
        Debug.Log($"UISafeAreaHelper: Применена безопасная зона для {gameObject.name} (тип: {elementType})");
    }
    
    /// <summary>
    /// Автоматически определяет тип UI элемента
    /// </summary>
    private void DetermineElementType()
    {
        if (GetComponent<Button>() != null)
        {
            elementType = ElementType.Button;
        }
        else if (GetComponent<Image>() != null && GetComponent<Button>() == null)
        {
            elementType = ElementType.Image;
        }
        else if (GetComponent<Text>() != null || GetComponent<TMPro.TextMeshProUGUI>() != null)
        {
            elementType = ElementType.Text;
        }
        else if (GetComponent<CanvasRenderer>() != null)
        {
            elementType = ElementType.Panel;
        }
        else
        {
            elementType = ElementType.Custom;
        }
    }
    
    /// <summary>
    /// Применяет безопасную зону для кнопок
    /// </summary>
    private void ApplyButtonSafeArea()
    {
        // Кнопки должны иметь отступы от краев
        float margin = marginPercent;
        
        // Проверяем позицию кнопки
        Vector2 currentAnchor = rectTransform.anchorMin;
        
        if (currentAnchor.x < 0.1f) // Левая сторона
        {
            rectTransform.anchorMin = new Vector2(margin, rectTransform.anchorMin.y);
        }
        else if (currentAnchor.x > 0.9f) // Правая сторона
        {
            rectTransform.anchorMax = new Vector2(1f - margin, rectTransform.anchorMax.y);
        }
        
        if (currentAnchor.y < 0.1f) // Нижняя сторона
        {
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, margin);
        }
        else if (currentAnchor.y > 0.9f) // Верхняя сторона
        {
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f - margin);
        }
    }
    
    /// <summary>
    /// Применяет безопасную зону для панелей
    /// </summary>
    private void ApplyPanelSafeArea()
    {
        // Панели должны занимать большую часть экрана с отступами
        float margin = marginPercent * 2f; // Больше отступов для панелей
        
        rectTransform.anchorMin = new Vector2(margin, margin);
        rectTransform.anchorMax = new Vector2(1f - margin, 1f - margin);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
    
    /// <summary>
    /// Применяет безопасную зону для текста
    /// </summary>
    private void ApplyTextSafeArea()
    {
        // Текст должен иметь небольшие отступы
        float margin = marginPercent * 0.5f; // Меньше отступов для текста
        
        Vector2 currentAnchor = rectTransform.anchorMin;
        
        if (currentAnchor.x < 0.05f) // Очень близко к левому краю
        {
            rectTransform.anchorMin = new Vector2(margin, rectTransform.anchorMin.y);
        }
        else if (currentAnchor.x > 0.95f) // Очень близко к правому краю
        {
            rectTransform.anchorMax = new Vector2(1f - margin, rectTransform.anchorMax.y);
        }
        
        if (currentAnchor.y < 0.05f) // Очень близко к нижнему краю
        {
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, margin);
        }
        else if (currentAnchor.y > 0.95f) // Очень близко к верхнему краю
        {
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f - margin);
        }
    }
    
    /// <summary>
    /// Применяет безопасную зону для изображений
    /// </summary>
    private void ApplyImageSafeArea()
    {
        // Изображения должны иметь умеренные отступы
        float margin = marginPercent;
        
        Vector2 currentAnchor = rectTransform.anchorMin;
        
        if (currentAnchor.x < 0.1f)
        {
            rectTransform.anchorMin = new Vector2(margin, rectTransform.anchorMin.y);
        }
        else if (currentAnchor.x > 0.9f)
        {
            rectTransform.anchorMax = new Vector2(1f - margin, rectTransform.anchorMax.y);
        }
        
        if (currentAnchor.y < 0.1f)
        {
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, margin);
        }
        else if (currentAnchor.y > 0.9f)
        {
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1f - margin);
        }
    }
    
    /// <summary>
    /// Применяет пользовательскую безопасную зону
    /// </summary>
    private void ApplyCustomSafeArea()
    {
        // Пользовательские элементы получают стандартные отступы
        float margin = marginPercent;
        
        rectTransform.anchorMin = new Vector2(
            Mathf.Max(rectTransform.anchorMin.x, margin),
            Mathf.Max(rectTransform.anchorMin.y, margin)
        );
        
        rectTransform.anchorMax = new Vector2(
            Mathf.Min(rectTransform.anchorMax.x, 1f - margin),
            Mathf.Min(rectTransform.anchorMax.y, 1f - margin)
        );
    }
    
    /// <summary>
    /// Восстанавливает оригинальные настройки
    /// </summary>
    [ContextMenu("Restore Original Settings")]
    public void RestoreOriginalSettings()
    {
        if (!isInitialized || rectTransform == null) return;
        
        rectTransform.anchorMin = new Vector2(originalAnchors.x, originalAnchors.y);
        rectTransform.anchorMax = new Vector2(originalAnchors.x, originalAnchors.y);
        rectTransform.offsetMin = originalOffsetMin;
        rectTransform.offsetMax = originalOffsetMax;
        
        Debug.Log($"UISafeAreaHelper: Восстановлены оригинальные настройки для {gameObject.name}");
    }
    
    /// <summary>
    /// Устанавливает новые отступы
    /// </summary>
    public void SetMargin(float newMargin)
    {
        marginPercent = Mathf.Clamp01(newMargin);
        if (isInitialized && applySafeArea)
        {
            ApplySafeArea();
        }
    }
    
    /// <summary>
    /// Включает/выключает применение безопасной зоны
    /// </summary>
    public void SetApplySafeArea(bool enabled)
    {
        applySafeArea = enabled;
        if (enabled && isInitialized)
        {
            ApplySafeArea();
        }
        else if (!enabled && isInitialized)
        {
            RestoreOriginalSettings();
        }
    }
    
    void OnValidate()
    {
        // В редакторе обновляем настройки при изменении параметров
        if (Application.isPlaying && isInitialized && applySafeArea)
        {
            ApplySafeArea();
        }
    }
} 