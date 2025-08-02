using UnityEngine;
using TMPro;

public class LocalizedTextComponent : MonoBehaviour
{
    [Header("Localization Settings")]
    public string localizationKey;
    public bool useFormatting = false;
    public object[] formatArgs;

    private TextMeshProUGUI textComponent;
    private LocalizationManager localizationManager;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            Debug.LogError($"LocalizedTextComponent на {gameObject.name} требует TextMeshProUGUI компонент!");
            // Отключаем компонент, чтобы избежать дальнейших ошибок
            enabled = false;
        }
    }

    void Start()
    {
        // Запускаем корутину для ожидания инициализации LocalizationManager
        StartCoroutine(WaitForLocalizationManager());
    }
    
    private System.Collections.IEnumerator WaitForLocalizationManager()
    {
        // Ждем до 30 кадров или пока LocalizationManager не будет готов
        int attempts = 0;
        while (LocalizationManager.Instance == null && attempts < 30)
        {
            yield return new WaitForEndOfFrame();
            attempts++;
        }
        
        localizationManager = LocalizationManager.Instance;
        if (localizationManager != null)
        {
            UpdateText();
        }
        else
        {
            Debug.LogWarning($"LocalizedTextComponent на {gameObject.name}: LocalizationManager.Instance недоступен после ожидания");
        }
    }

    public void UpdateText()
    {
        if (textComponent == null)
        {
            Debug.LogWarning($"LocalizedTextComponent на {gameObject.name}: TextMeshProUGUI компонент отсутствует");
            return;
        }
        
        // Попробуем получить LocalizationManager, если он еще не установлен
        if (localizationManager == null)
        {
            localizationManager = LocalizationManager.Instance;
        }
        
        if (localizationManager == null)
        {
            // Не выводим предупреждение, так как это нормально во время инициализации
            return;
        }
        
        if (string.IsNullOrEmpty(localizationKey))
        {
            Debug.LogWarning($"LocalizedTextComponent на {gameObject.name}: ключ локализации не установлен");
            return;
        }
        
        try
        {
            if (useFormatting && formatArgs != null && formatArgs.Length > 0)
            {
                string formattedText = localizationManager.GetText(localizationKey, formatArgs);
                textComponent.text = formattedText;
                Debug.Log($"LocalizedTextComponent {gameObject.name}: форматированный текст '{formattedText}' с параметрами [{string.Join(", ", formatArgs)}]");
            }
            else
            {
                string plainText = localizationManager.GetText(localizationKey);
                textComponent.text = plainText;
                
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при обновлении текста в {gameObject.name}: {e.Message}");
        }
    }

    public void SetFormatArgs(params object[] args)
    {
        formatArgs = args;
        useFormatting = true; // Автоматически включаем форматирование при установке параметров
        UpdateText();
    }

    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }
    
    // Метод для принудительного обновления текста, если LocalizationManager стал доступен позже
    public void ForceUpdate()
    {
        if (localizationManager == null)
        {
            localizationManager = LocalizationManager.Instance;
        }
        UpdateText();
    }
} 