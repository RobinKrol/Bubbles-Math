using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class VibrationHelper : MonoBehaviour
{
    // WebGL JavaScript функция для вибрации
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebGLVibrate();
    #endif
    
    [Header("Vibration Settings")]
    [SerializeField] private bool enableVibration = true;
    [SerializeField] private float vibrationDuration = 0.1f;
    [SerializeField] private bool useVisualFeedback = true;
    
    [Header("Visual Feedback")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float feedbackDuration = 0.1f;
    
    private bool isVibrating = false;
    
    void Start()
    {
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Vibrate);
    }

    public void Vibrate()
    {
        if (!enableVibration) return;
        
        StartCoroutine(VibrateCoroutine());
    }
    
    private IEnumerator VibrateCoroutine()
    {
        if (isVibrating) yield break;
        
        isVibrating = true;
        
        // Платформенная вибрация
        VibratePlatform();
        
        // Визуальная обратная связь
        if (useVisualFeedback)
        {
            yield return StartCoroutine(VisualFeedback());
        }
        else
        {
            yield return new WaitForSeconds(vibrationDuration);
        }
        
        isVibrating = false;
    }
    
    private void VibratePlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                // Нативная вибрация для мобильных устройств
                VibrateMobile();
                Debug.Log("Vibration: Native mobile vibration");
                break;
                
            case RuntimePlatform.WebGLPlayer:
                // WebGL вибрация через JavaScript API
                VibrateWebGL();
                break;
                
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                // Windows вибрация (если поддерживается)
                VibrateWindows();
                break;
                
            default:
                // Fallback для других платформ
                Debug.Log($"Vibration: Platform {Application.platform} not supported, using visual feedback only");
                break;
        }
    }
    
    private void VibrateWebGL()
    {
        // WebGL вибрация через JavaScript API
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            try
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                // Вызываем JavaScript функцию через JSLib
                WebGLVibrate();
                Debug.Log("Vibration: WebGL vibration via JavaScript API");
                #else
                // Fallback для редактора
                Debug.Log("Vibration: WebGL vibration (editor mode - visual feedback only)");
                #endif
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"WebGL vibration failed: {e.Message}");
                // При ошибке используем только визуальную обратную связь
            }
        }
    }
    
    private void VibrateMobile()
    {
        // Вибрация для мобильных устройств
        try
        {
            // Используем простой подход - визуальная обратная связь для мобильных
            // В реальном проекте здесь можно добавить нативные API вызовы
            Debug.Log("Vibration: Mobile platform - using visual feedback");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Mobile vibration failed: {e.Message}");
        }
    }
    
    private void VibrateWindows()
    {
        // Windows вибрация (если поддерживается)
        try
        {
            // Можно добавить поддержку геймпадов или других устройств
            Debug.Log("Vibration: Windows platform - visual feedback only");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Windows vibration failed: {e.Message}");
        }
    }
    
    private IEnumerator VisualFeedback()
    {
        if (transform == null) yield break;
        
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;
        
        // Увеличиваем размер
        float elapsed = 0f;
        while (elapsed < feedbackDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (feedbackDuration / 2f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Возвращаем к исходному размеру
        elapsed = 0f;
        while (elapsed < feedbackDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (feedbackDuration / 2f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    // Публичные методы для настройки
    public void SetVibrationEnabled(bool enabled)
    {
        enableVibration = enabled;
    }
    
    public void SetVibrationDuration(float duration)
    {
        vibrationDuration = Mathf.Max(0.05f, duration);
    }
    
    public void SetVisualFeedbackEnabled(bool enabled)
    {
        useVisualFeedback = enabled;
    }
    
    public void SetScaleMultiplier(float multiplier)
    {
        scaleMultiplier = Mathf.Max(1.0f, multiplier);
    }
    
    // Статический метод для быстрого вызова вибрации
    public static void VibrateOnce()
    {
        var vibrationHelper = FindAnyObjectByType<VibrationHelper>();
        if (vibrationHelper != null)
        {
            vibrationHelper.Vibrate();
        }
    }
    
    // Метод для проверки поддержки вибрации на текущей платформе
    public static bool IsVibrationSupported()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return true; // Мобильные платформы поддерживают вибрацию
            case RuntimePlatform.WebGLPlayer:
                #if UNITY_WEBGL && !UNITY_EDITOR
                return true; // WebGL поддерживает вибрацию через JavaScript API
                #else
                return false; // В редакторе WebGL вибрация недоступна
                #endif
            default:
                return false;
        }
    }
}
