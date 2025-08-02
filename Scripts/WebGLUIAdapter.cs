using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Автоматически настраивает Canvas Scaler для WebGL платформы
/// для обеспечения корректного отображения UI при изменении размера окна браузера
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class WebGLUIAdapter : MonoBehaviour
{
    [Header("WebGL UI Settings")]
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
    [SerializeField] private CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    [SerializeField] private float matchWidthOrHeight = 0.5f;
    
    [Header("Safe Area Settings")]
    [SerializeField] private bool useSafeArea = true;
    [SerializeField] private float safeAreaMargin = 50f;
    [SerializeField] private bool autoDetectOrientation = true;
    
    [Header("Performance Settings")]
    [SerializeField] private float screenSizeCheckInterval = 0.1f; // Уменьшено для более быстрой реакции
    [SerializeField] private bool enableDebugLogs = true;
    
    [Header("Advanced Settings")]
    [SerializeField] private bool forceCanvasUpdate = true;
    [SerializeField] private bool useDynamicScaling = true;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    private RectTransform canvasRectTransform;
    private Vector2 lastScreenSize;
    private bool isInitialized = false;
    private float lastAspectRatio;
    
    void Awake()
    {
        // Получаем компоненты
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
        canvasRectTransform = GetComponent<RectTransform>();
        
        // Инициализируем размер экрана
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastAspectRatio = (float)Screen.width / Screen.height;
        
        // Настраиваем Canvas Scaler для WebGL
        SetupCanvasScalerForWebGL();
        
        // Настраиваем Safe Area
        if (useSafeArea)
        {
            SetupSafeArea();
        }
        
        isInitialized = true;
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Инициализация завершена для {gameObject.name}");
        }
    }
    
    void Start()
    {
        // Подписываемся на изменение размера экрана с более частой проверкой
        InvokeRepeating(nameof(CheckScreenSize), 0.05f, screenSizeCheckInterval);
        
        // Принудительно обновляем Canvas после инициализации
        if (forceCanvasUpdate)
        {
            Canvas.ForceUpdateCanvases();
        }
    }
    
    /// <summary>
    /// Настраивает Canvas Scaler для WebGL платформы
    /// </summary>
    private void SetupCanvasScalerForWebGL()
    {
        if (canvasScaler == null) return;
        
        // Автоматически определяем эталонное разрешение на основе ориентации
        if (autoDetectOrientation)
        {
            DetermineOptimalReferenceResolution();
        }
        
        // Устанавливаем режим масштабирования
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        
        // Устанавливаем эталонное разрешение
        canvasScaler.referenceResolution = referenceResolution;
        
        // Устанавливаем режим соответствия экрана
        canvasScaler.screenMatchMode = screenMatchMode;
        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        
        // Устанавливаем другие параметры
        canvasScaler.referencePixelsPerUnit = 100f;
        
        // Применяем динамическое масштабирование
        if (useDynamicScaling)
        {
            ApplyDynamicScaling();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Canvas Scaler настроен для WebGL с разрешением {referenceResolution}");
        }
    }
    
    /// <summary>
    /// Применяет динамическое масштабирование на основе размера экрана
    /// </summary>
    private void ApplyDynamicScaling()
    {
        if (canvasScaler == null) return;
        
        float currentAspectRatio = (float)Screen.width / Screen.height;
        float referenceAspectRatio = referenceResolution.x / referenceResolution.y;
        
        // Вычисляем оптимальный matchWidthOrHeight на основе соотношения сторон
        float optimalMatch;
        
        if (currentAspectRatio > referenceAspectRatio)
        {
            // Экран шире эталонного - масштабируем по высоте
            optimalMatch = 1.0f;
        }
        else if (currentAspectRatio < referenceAspectRatio)
        {
            // Экран уже эталонного - масштабируем по ширине
            optimalMatch = 0.0f;
        }
        else
        {
            // Соотношение сторон одинаковое - используем настройку по умолчанию
            optimalMatch = matchWidthOrHeight;
        }
        
        // Ограничиваем масштаб
        float scaleFactor = Mathf.Clamp(optimalMatch, minScale, maxScale);
        canvasScaler.matchWidthOrHeight = scaleFactor;
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Применено динамическое масштабирование. Match: {scaleFactor:F2}, Aspect: {currentAspectRatio:F2}");
        }
    }
    
    /// <summary>
    /// Автоматически определяет оптимальное эталонное разрешение
    /// </summary>
    private void DetermineOptimalReferenceResolution()
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        
        if (aspectRatio > 1.5f) // Широкоформатный экран
        {
            referenceResolution = new Vector2(1920, 1080);
        }
        else if (aspectRatio > 1.2f) // Стандартный 16:10
        {
            referenceResolution = new Vector2(1680, 1050);
        }
        else if (aspectRatio > 0.8f) // Квадратный или близкий к квадратному
        {
            referenceResolution = new Vector2(1080, 1080);
        }
        else // Вертикальная ориентация
        {
            referenceResolution = new Vector2(1080, 1920);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Определено оптимальное разрешение {referenceResolution} для соотношения {aspectRatio:F2}");
        }
    }
    
    /// <summary>
    /// Настраивает Safe Area для предотвращения обрезания UI
    /// </summary>
    private void SetupSafeArea()
    {
        if (canvasRectTransform == null) return;
        
        // Получаем размеры экрана
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        
        // Вычисляем Safe Area
        Rect safeArea = Screen.safeArea;
        
        // Применяем Safe Area к Canvas
        canvasRectTransform.anchorMin = Vector2.zero;
        canvasRectTransform.anchorMax = Vector2.one;
        canvasRectTransform.offsetMin = Vector2.zero;
        canvasRectTransform.offsetMax = Vector2.zero;
        
        // Добавляем дополнительные отступы для WebGL
        float marginX = safeAreaMargin / screenSize.x;
        float marginY = safeAreaMargin / screenSize.y;
        
        // Ограничиваем отступы разумными значениями
        marginX = Mathf.Clamp(marginX, 0f, 0.1f); // Уменьшено с 0.2f до 0.1f
        marginY = Mathf.Clamp(marginY, 0f, 0.1f); // Уменьшено с 0.2f до 0.1f
        
        canvasRectTransform.anchorMin = new Vector2(marginX, marginY);
        canvasRectTransform.anchorMax = new Vector2(1f - marginX, 1f - marginY);
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Safe Area настроен с отступами {safeAreaMargin}px (X: {marginX:F3}, Y: {marginY:F3})");
        }
    }
    
    /// <summary>
    /// Проверяет изменение размера экрана и обновляет UI
    /// </summary>
    private void CheckScreenSize()
    {
        if (!isInitialized) return;
        
        // Проверяем, изменился ли размер экрана или соотношение сторон
        Vector2 newScreenSize = new Vector2(Screen.width, Screen.height);
        float newAspectRatio = (float)Screen.width / Screen.height;
        
        bool sizeChanged = lastScreenSize.x != newScreenSize.x || lastScreenSize.y != newScreenSize.y;
        bool aspectChanged = Mathf.Abs(newAspectRatio - lastAspectRatio) > 0.01f;
        
        if (sizeChanged || aspectChanged)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"WebGLUIAdapter: Размер экрана изменился с {lastScreenSize} на {newScreenSize}, соотношение: {newAspectRatio:F2}");
            }
            
            lastScreenSize = newScreenSize;
            lastAspectRatio = newAspectRatio;
            
            // Переопределяем эталонное разрешение если включена автодетекция
            if (autoDetectOrientation && aspectChanged)
            {
                DetermineOptimalReferenceResolution();
            }
            
            // Обновляем Canvas Scaler
            SetupCanvasScalerForWebGL();
            
            // Обновляем Safe Area
            if (useSafeArea)
            {
                SetupSafeArea();
            }
            
            // Принудительно обновляем Canvas
            if (forceCanvasUpdate)
            {
                Canvas.ForceUpdateCanvases();
                
                // Дополнительное обновление через кадр
                StartCoroutine(ForceUpdateNextFrame());
            }
            
            // Уведомляем другие компоненты об изменении размера
            OnScreenSizeChanged?.Invoke(newScreenSize);
        }
    }
    
    /// <summary>
    /// Принудительно обновляет Canvas в следующем кадре
    /// </summary>
    private System.Collections.IEnumerator ForceUpdateNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
    }
    
    /// <summary>
    /// Событие изменения размера экрана
    /// </summary>
    public System.Action<Vector2> OnScreenSizeChanged;
    
    /// <summary>
    /// Принудительно обновляет настройки UI
    /// </summary>
    [ContextMenu("Force Update UI")]
    public void ForceUpdateUI()
    {
        SetupCanvasScalerForWebGL();
        if (useSafeArea)
        {
            SetupSafeArea();
        }
        
        if (forceCanvasUpdate)
        {
            Canvas.ForceUpdateCanvases();
            StartCoroutine(ForceUpdateNextFrame());
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"WebGLUIAdapter: Принудительное обновление UI выполнено");
        }
    }
    
    /// <summary>
    /// Устанавливает новые параметры и обновляет UI
    /// </summary>
    public void UpdateSettings(Vector2 newReferenceResolution, CanvasScaler.ScreenMatchMode newScreenMatchMode, float newMatchWidthOrHeight)
    {
        referenceResolution = newReferenceResolution;
        screenMatchMode = newScreenMatchMode;
        matchWidthOrHeight = newMatchWidthOrHeight;
        
        ForceUpdateUI();
    }
    
    /// <summary>
    /// Получает текущие настройки
    /// </summary>
    public (Vector2 resolution, CanvasScaler.ScreenMatchMode matchMode, float matchValue) GetCurrentSettings()
    {
        return (referenceResolution, screenMatchMode, matchWidthOrHeight);
    }
    
    /// <summary>
    /// Включает/выключает отладочные сообщения
    /// </summary>
    public void SetDebugLogs(bool enabled)
    {
        enableDebugLogs = enabled;
    }
    
    void OnValidate()
    {
        // В редакторе обновляем настройки при изменении параметров
        if (Application.isPlaying && isInitialized)
        {
            ForceUpdateUI();
        }
    }
    
    void OnDestroy()
    {
        // Отменяем повторяющиеся вызовы
        CancelInvoke(nameof(CheckScreenSize));
    }
} 