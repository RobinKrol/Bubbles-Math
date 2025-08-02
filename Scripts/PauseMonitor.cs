using UnityEngine;

public class PauseMonitor : MonoBehaviour
{
    [Header("Pause Monitoring")]
    [SerializeField] private bool enableMonitoring = true;
    [SerializeField] private float checkInterval = 1f;
    
    private float lastCheckTime = 0f;
    private bool lastPauseState = false;
    private float lastTimeScale = 1f;
    
    void Update()
    {
        if (!enableMonitoring) return;
        
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckPauseState();
            lastCheckTime = Time.time;
        }
    }
    
    private void CheckPauseState()
    {
        bool currentPauseState = Bubbles.IsPaused;
        float currentTimeScale = Time.timeScale;
        
        // Проверяем несоответствия
        if (currentPauseState != lastPauseState)
        {
            Debug.Log($"PauseMonitor: Bubbles.IsPaused changed from {lastPauseState} to {currentPauseState}");
            lastPauseState = currentPauseState;
        }
        
        if (Mathf.Abs(currentTimeScale - lastTimeScale) > 0.01f)
        {
            Debug.Log($"PauseMonitor: Time.timeScale changed from {lastTimeScale} to {currentTimeScale}");
            lastTimeScale = currentTimeScale;
        }
        
        // Проверяем логическое несоответствие
        if (currentPauseState && currentTimeScale > 0.01f)
        {
            Debug.LogWarning($"PauseMonitor: WARNING - Game is paused (IsPaused=true) but Time.timeScale={currentTimeScale}");
        }
        
        if (!currentPauseState && currentTimeScale < 0.01f)
        {
            Debug.LogWarning($"PauseMonitor: WARNING - Game is not paused (IsPaused=false) but Time.timeScale={currentTimeScale}");
        }
    }
    
    [ContextMenu("Force Pause State Check")]
    public void ForcePauseStateCheck()
    {
        CheckPauseState();
    }
    
    [ContextMenu("Log Current Pause State")]
    public void LogCurrentPauseState()
    {
        Debug.Log($"PauseMonitor: Current state - Bubbles.IsPaused: {Bubbles.IsPaused}, Time.timeScale: {Time.timeScale}");
    }
    
    [ContextMenu("Fix Pause State")]
    public void FixPauseState()
    {
        // Находим GameModeManager и исправляем состояние
        GameModeManager gameManager = FindAnyObjectByType<GameModeManager>();
        if (gameManager != null)
        {
            // Используем рефлексию для доступа к приватному полю isManuallyPaused
            var field = typeof(GameModeManager).GetField("isManuallyPaused", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                bool isManuallyPaused = (bool)field.GetValue(gameManager);
                Debug.Log($"PauseMonitor: GameModeManager.isManuallyPaused = {isManuallyPaused}");
                
                // Исправляем Time.timeScale
                Time.timeScale = isManuallyPaused ? 0f : 1f;
                Bubbles.IsPaused = isManuallyPaused;
                
                Debug.Log($"PauseMonitor: Fixed - Time.timeScale: {Time.timeScale}, Bubbles.IsPaused: {Bubbles.IsPaused}");
            }
        }
        else
        {
            Debug.LogError("PauseMonitor: GameModeManager not found!");
        }
    }
} 