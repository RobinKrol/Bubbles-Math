using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Обработчик изменения размера окна браузера для WebGL
/// Принудительно обновляет UI при изменении размера окна
/// </summary>
public class UIResizeHandler : MonoBehaviour
{
    [Header("Resize Settings")]
    [SerializeField] private bool enableResizeHandling = true;
    [SerializeField] private float resizeCheckInterval = 0.05f;
    [SerializeField] private bool forceCanvasUpdate = true;
    [SerializeField] private bool updateAllCanvases = true;
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool logResizeEvents = true;
    
    private Vector2 lastWindowSize;
    private bool isInitialized = false;
    
    void Awake()
    {
        // Инициализируем размер окна
        lastWindowSize = new Vector2(Screen.width, Screen.height);
        isInitialized = true;
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIResizeHandler: Инициализация завершена для {gameObject.name}");
        }
    }
    
    void Start()
    {
        if (enableResizeHandling)
        {
            // Запускаем проверку изменения размера
            InvokeRepeating(nameof(CheckWindowResize), 0.1f, resizeCheckInterval);
        }
    }
    
    /// <summary>
    /// Проверяет изменение размера окна браузера
    /// </summary>
    private void CheckWindowResize()
    {
        if (!isInitialized || !enableResizeHandling) return;
        
        Vector2 currentWindowSize = new Vector2(Screen.width, Screen.height);
        
        // Проверяем, изменился ли размер окна
        if (lastWindowSize.x != currentWindowSize.x || lastWindowSize.y != currentWindowSize.y)
        {
            if (logResizeEvents)
            {
                Debug.Log($"UIResizeHandler: Размер окна изменился с {lastWindowSize} на {currentWindowSize}");
            }
            
            lastWindowSize = currentWindowSize;
            
            // Обрабатываем изменение размера
            HandleWindowResize(currentWindowSize);
        }
    }
    
    /// <summary>
    /// Обрабатывает изменение размера окна
    /// </summary>
    private void HandleWindowResize(Vector2 newSize)
    {
        // Принудительно обновляем все Canvas
        if (forceCanvasUpdate)
        {
            if (updateAllCanvases)
            {
                Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (Canvas canvas in allCanvases)
                {
                    if (canvas != null)
                    {
                        Canvas.ForceUpdateCanvases();
                        break; // Достаточно вызвать один раз для всех Canvas
                    }
                }
            }
            else
            {
                Canvas.ForceUpdateCanvases();
            }
            
            // Дополнительное обновление через кадр
            StartCoroutine(ForceUpdateNextFrame());
        }
        
        // Уведомляем другие компоненты
        OnWindowResized?.Invoke(newSize);
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIResizeHandler: Обработка изменения размера окна завершена");
        }
    }
    
    /// <summary>
    /// Принудительно обновляет Canvas в следующем кадре
    /// </summary>
    private System.Collections.IEnumerator ForceUpdateNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        
        // Дополнительное обновление через еще один кадр для надежности
        yield return null;
        Canvas.ForceUpdateCanvases();
    }
    
    /// <summary>
    /// Событие изменения размера окна
    /// </summary>
    public System.Action<Vector2> OnWindowResized;
    
    /// <summary>
    /// Принудительно обрабатывает изменение размера
    /// </summary>
    [ContextMenu("Force Handle Resize")]
    public void ForceHandleResize()
    {
        Vector2 currentSize = new Vector2(Screen.width, Screen.height);
        HandleWindowResize(currentSize);
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIResizeHandler: Принудительная обработка изменения размера выполнена");
        }
    }
    
    /// <summary>
    /// Включает/выключает обработку изменения размера
    /// </summary>
    public void SetResizeHandling(bool enabled)
    {
        enableResizeHandling = enabled;
        
        if (enabled && !IsInvoking(nameof(CheckWindowResize)))
        {
            InvokeRepeating(nameof(CheckWindowResize), 0.1f, resizeCheckInterval);
        }
        else if (!enabled && IsInvoking(nameof(CheckWindowResize)))
        {
            CancelInvoke(nameof(CheckWindowResize));
        }
    }
    
    /// <summary>
    /// Получает текущий размер окна
    /// </summary>
    public Vector2 GetCurrentWindowSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }
    
    void OnValidate()
    {
        // В редакторе обновляем настройки при изменении параметров
        if (Application.isPlaying && isInitialized)
        {
            SetResizeHandling(enableResizeHandling);
        }
    }
    
    void OnDestroy()
    {
        // Отменяем повторяющиеся вызовы
        CancelInvoke(nameof(CheckWindowResize));
    }
} 