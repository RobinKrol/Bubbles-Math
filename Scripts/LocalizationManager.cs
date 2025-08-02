using UnityEngine;
using TMPro;
using YG;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class LocalizationManager : MonoBehaviour
{
    [System.Serializable]
    public class LocalizedText
    {
        public string key;
        public string russian;
        public string english;
    }

    [Header("Localization Data")]
    public List<LocalizedText> localizedTexts = new List<LocalizedText>();

    [Header("UI References")]
    public TextMeshProUGUI[] textsToLocalize;

    private Dictionary<string, LocalizedText> textDictionary = new Dictionary<string, LocalizedText>();
    private string currentLanguage = "en";
    public static LocalizationManager Instance { get; private set; }
    
    // Событие для уведомления о смене языка
    public static event Action<string> OnLanguageChanged;
    
    // Событие для уведомления о необходимости обновления текстов
    public static event Action OnTextsUpdateRequested;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        YG2.onSwitchLang += OnLanguageReceived;
        YG2.GetLanguage();
    }
    
    void OnDestroy()
    {
        YG2.onSwitchLang -= OnLanguageReceived;
    }

    private void InitializeLocalization()
    {
        InitializeDefaultTexts();
        foreach (var text in localizedTexts)
        {
            textDictionary[text.key] = text;
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Обновляем тексты с задержкой для корректной инициализации
        StartCoroutine(UpdateAllTextsDelayed());
    }

    private void InitializeDefaultTexts()
    {
        localizedTexts.Clear();

        AddText("continue_game_title", "Продолжить игру?", "Continue Game?");
        AddText("continue_game_description", "Посмотрите рекламу для продолжения игры и сохранения вашего прогресса!", "Watch an ad to continue the game and save your score!");
        AddText("continue_game_button", "Продолжить игру", "Continue Game");
        AddText("close_button", "Закрыть", "Close");
        AddText("your_score", "Ваш счет: {0}", "Your Score: {0}");

        AddText("correct_answer", "Верно!", "Correct!");
        AddText("wrong_answer", "Ошибка!", "Wrong!");

        AddText("play_button", "Играть", "Play");
        AddText("settings_button", "Настройки", "Settings");
        AddText("leaderboard_button", "Таблица лидеров", "Leaderboard");
        AddText("menu_button", "Меню", "Menu");
        AddText("HowToPlay_button", "Как играть?", "HowToPlay?");

        AddText("pause_button", "Пауза", "Pause");
        AddText("continue_button", "Продолжить", "Continue");

        AddText("score_label", "Счет:", "Score:");
        AddText("level_label", "Уровень", "Level");
        AddText("operatin_label", "Операция:", "Operation:");
        AddText("current_label", "Равно:", "Equals:");

        AddText("ad_error", "Ошибка показа рекламы", "Error showing advertisement");
        AddText("game_over", "Игра окончена!", "Game Over!");
        AddText("new_high_score", "Новый рекорд!", "New High Score!");

        AddText("easy_mode", "Легкий", "Easy");
        AddText("normal_mode", "Нормальный", "Normal");
        AddText("hard_mode", "Трудный", "Hard");
        AddText("expert_mode", "Экперт", "Expert");
        AddText("master_mode", "Мастер", "Master");

        // Tutorial steps
        AddText("tutorial_step_1", "Это Главное число. Твоя задача — получить это число, используя предложенные операции и цифры.", "This is the Main Number. Your goal is to reach this number using the given operations and digits.");
        AddText("tutorial_step_2", "Это Текущее значение. С него ты начинаешь каждый раунд. Меняй его, чтобы получить главное число!", "This is the Current Total. You start each round with this value. Change it to reach the main number!");
        AddText("tutorial_step_3", "Вот операция (например, +, -, ×, ÷). Используй её вместе с выбранной цифрой, чтобы изменить текущее значение.", "Here is the operation (for example, +, -, ×, ÷). Use it with the chosen digit to change the current total.");
        AddText("tutorial_step_4", "Выбирай пузырёк с цифрой, чтобы применить операцию к текущему значению. Только один пузырёк — правильный!", "Choose a bubble with a digit to apply the operation to the current total. Only one bubble is correct!");
        AddText("tutorial_step_5", "За каждый правильный ответ ты получаешь очки. Старайся побить свой рекорд!", "You earn points for each correct answer. Try to beat your high score!");
        AddText("tutorial_step_6", "Следи за таймером! Если время закончится — начнётся новый раунд.", "Watch the timer! If time runs out, a new round will start.");
        AddText("tutorial_step_7", "Если ошибёшься — раунд закончится, но ты сможешь продолжить с сохранённым счётом.", "If you make a mistake, the round will end, but you can continue with your saved score.");
    }

    private void AddText(string key, string russian, string english)
    {
        localizedTexts.Add(new LocalizedText
        {
            key = key,
            russian = russian,
            english = english
        });
    }

    private void OnLanguageReceived(string language)
    {
        currentLanguage = language;
        Debug.Log($"Language received: {language}");
        
        // Уведомляем о смене языка
        OnLanguageChanged?.Invoke(language);
        
        // Запускаем обновление текстов с небольшой задержкой
        StartCoroutine(UpdateAllTextsDelayed());
    }
    
    private System.Collections.IEnumerator UpdateAllTextsDelayed()
    {
        // Ждем несколько кадров, чтобы все компоненты успели инициализироваться
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        
        UpdateAllTexts();
    }

    public string GetText(string key, params object[] args)
    {
        if (textDictionary.TryGetValue(key, out LocalizedText text))
        {
            string localizedString = currentLanguage == "ru" ? text.russian : text.english;
            if (args.Length > 0)
            {
                return string.Format(localizedString, args);
            }
            return localizedString;
        }
        Debug.LogWarning($"Localization key not found: {key}");
        return key;
    }

    public void UpdateAllTexts()
    {
        // Обновляем тексты из массива
        foreach (var textComponent in textsToLocalize)
        {
            if (textComponent != null)
            {
                LocalizedTextComponent localizedComponent = textComponent.GetComponent<LocalizedTextComponent>();
                if (localizedComponent != null)
                {
                    localizedComponent.UpdateText();
                }
            }
        }
        
        // Находим все LocalizedTextComponent в сцене и обновляем их
        LocalizedTextComponent[] allLocalizedComponents = FindObjectsByType<LocalizedTextComponent>(FindObjectsSortMode.None);
        foreach (var component in allLocalizedComponents)
        {
            if (component != null && component.gameObject.activeInHierarchy)
            {
                component.UpdateText();
            }
        }
        
        // Уведомляем о необходимости обновления текстов
        OnTextsUpdateRequested?.Invoke();
    }
    
    // Метод для принудительного обновления всех текстов
    public void ForceUpdateAllTexts()
    {
        UpdateAllTexts();
    }
    
    // Метод для проверки готовности системы локализации
    public bool IsReady()
    {
        return Instance != null && !string.IsNullOrEmpty(currentLanguage);
    }
    
    // Метод для принудительного обновления всех текстов с проверкой готовности
    public void SafeUpdateAllTexts()
    {
        if (!IsReady())
        {
            Debug.LogWarning("LocalizationManager не готов, пропускаем обновление текстов");
            return;
        }
        
        UpdateAllTexts();
    }
    
    // Метод для обновления текстов в конкретном GameObject и его дочерних объектах
    public void UpdateTextsInGameObject(GameObject targetObject)
    {
        if (targetObject == null) return;
        
        LocalizedTextComponent[] components = targetObject.GetComponentsInChildren<LocalizedTextComponent>(true);
        foreach (var component in components)
        {
            if (component != null)
            {
                component.UpdateText();
            }
        }
    }
    
    public void SwitchLanguage(string language)
    {
        YG2.SwitchLanguage(language);
    }

    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }
}
