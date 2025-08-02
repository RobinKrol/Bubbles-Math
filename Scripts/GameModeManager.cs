using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;


public class GameModeManager : MonoBehaviour
{ 
    [Header("Managers")]
    public MainNumberManager mainNumberManager;
    public OperationManager operationManager;
    public TimeBarController timeBar;
    public GameTimerController gameTimer;
    public BubbleSpawner bubbleSpawner;
    public GameMode gameMode;
    public LeaderboardManager leaderboardManager;
    public RewardedAdManager rewardedAdManager;

    [Header("UI")]
    public GameObject gameUI;
    public TextMeshProUGUI currentTotalText; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI yourScoreText;
    public GameObject leaderboardPanel;
    
    [Header("Answer Texts")]
    public TextMeshProUGUI rightAnswerText;
    public TextMeshProUGUI wrongAnswerText;
    public AnswerTextAnimation rightAnswerAnimation;
    public AnswerTextAnimation wrongAnswerAnimation;

    [Header("Pause")]
    public GameObject pausePanel;      // Панель паузы
    public Button pauseButton;         // Кнопка паузы

    [Header("Variables")]
    public int currentTotal;
    public int digit; // Текущий Digit для раунда
 
    [Header("Score")]
    public int score = 0;
    public int highScore = 0;
    private int savedScore = 0; // Сохраняем счет для продолжения игры
    
    

    private GameModeType currentMode = GameModeType.Easy;
    private bool isManuallyPaused = false;


    void Awake()
    {
        gameTimer.OnGameTimerEnd += EndGame;
        Bubbles.IsPaused = false; // Сбросить флаг паузы для пузырей
        Time.timeScale = 1f;      // Сбросить паузу на всякий случай
        
    }
    void Start()
    {
         
        YG.YG2.StickyAdActivity(false); // Скрыть баннер во время игры

        if (pauseButton != null)
        pauseButton.onClick.AddListener(PauseGame);

        if (pausePanel != null)
        pausePanel.SetActive(false); // Скрыть панель паузы по умолчанию
        
        // Очищаем сохраненный счет для продолжения игры при начале новой игры
        PlayerPrefs.DeleteKey("SavedScoreForContinue");
        PlayerPrefs.Save();
        
        // Загружаем сохраненный счет или начинаем с 0
        score = PlayerPrefs.GetInt("CurrentScore", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Дополнительная проверка: если счет не 0, но нет сохраненного счета для продолжения,
        // то это может быть остаток от предыдущей сессии - сбрасываем
        if (score > 0 && !PlayerPrefs.HasKey("SavedScoreForContinue"))
        {
            Debug.Log($"Обнаружен остаточный счет {score}, сбрасываем к 0");
            score = 0;
            PlayerPrefs.SetInt("CurrentScore", 0);
            PlayerPrefs.Save();
        }
        
        Debug.Log($"Start: загружен счет {score}, рекорд {highScore}, очищен SavedScoreForContinue");
        AudioManager.Instance.PlayMusic();
        UpdateScoreUI();
        NextRound();
        timeBar.OnRoundEnd += OnTimeBarEnd;
    }
    public void PauseGame()
{
    Debug.Log("PauseGame called");
    Time.timeScale = 0f;
    isManuallyPaused = true;
    if (pausePanel != null)
        pausePanel.SetActive(true);

    Bubbles.IsPaused = true;
    YG.YG2.StickyAdActivity(true); // Показать баннер
    
    Debug.Log($"Game paused - Time.timeScale: {Time.timeScale}, isManuallyPaused: {isManuallyPaused}, Bubbles.IsPaused: {Bubbles.IsPaused}");
}

public void ResumeGame()
{
    Debug.Log("ResumeGame called");
    Time.timeScale = 1f;
    isManuallyPaused = false;
    if (pausePanel != null)
        pausePanel.SetActive(false);
    
    Bubbles.IsPaused = false;
    YG.YG2.StickyAdActivity(false); // Скрыть баннер
    
    Debug.Log($"Game resumed - Time.timeScale: {Time.timeScale}, isManuallyPaused: {isManuallyPaused}, Bubbles.IsPaused: {Bubbles.IsPaused}");
}

     public void NextRound()
    {
        Debug.Log("NextRound called! Stack: " + System.Environment.StackTrace);
        
        // Сбрасываем анимации текстов ответов перед новым раундом
       // if (rightAnswerAnimation != null)
       //     rightAnswerAnimation.ResetAnimation();
        if (wrongAnswerAnimation != null)
            wrongAnswerAnimation.ResetAnimation();
        
        var modeParams = GetCurrentParams();
        operationManager.GenerateOperation();
        string op = operationManager.currentOperation;
        
        // Генерируем Digit для раунда
        if (op == "*" || op == "/")
            digit = Random.Range(modeParams.digitMin, modeParams.digitMax + 1);
        else
            digit = Random.Range(modeParams.digitMin, modeParams.digitMax + 1);

        
        // Генерируем MainNumber и CurrentTotal
        int mainNumber = 0;
        do {
            switch (op)
            {
                case "+":
                    mainNumber = Random.Range(modeParams.plusRange.min, modeParams.plusRange.max + 1);
                    currentTotal = mainNumber - digit;
                    break;
                case "-":
                    mainNumber = Random.Range(modeParams.minusRange.min, modeParams.minusRange.max + 1);
                    currentTotal = mainNumber + digit;
                    break;
                case "*":
                    currentTotal = 2;
                    digit = Random.Range(modeParams.digitMin, modeParams.digitMax + 1);
                    mainNumber = currentTotal * digit;
                    OperationRange mulRange = modeParams.multiplyRange;
                    if (mainNumber < mulRange.min || mainNumber > mulRange.max)
                    {
                        List<int> possibleDigits = new List<int>();
                        for (int d = modeParams.digitMin; d <= modeParams.digitMax; d++)
                        {
                            int candidate = 2 * d;
                            if (candidate >= mulRange.min && candidate <= mulRange.max)
                                possibleDigits.Add(d);
                        }
                        if (possibleDigits.Count > 0)
                        {
                            digit = possibleDigits[Random.Range(0, possibleDigits.Count)];
                            mainNumber = 2 * digit;
                        }
                        else
                        {
                            List<int> possibleMainNumbers = new List<int>();
                            for (int n = mulRange.min; n <= mulRange.max; n++)
                                if (n % digit == 0) possibleMainNumbers.Add(n);
                            mainNumber = possibleMainNumbers[Random.Range(0, possibleMainNumbers.Count)];
                            currentTotal = mainNumber / digit;
                        }
                    }
                    break;
                case "/":
                    OperationRange divRange = modeParams.divideRange;
                    mainNumber = Random.Range(divRange.min, divRange.max + 1);
                    currentTotal = mainNumber * digit;
                    break;
            }
        } while (mainNumber == digit);

        mainNumberManager.SetMainNumber(mainNumber);
        
        Debug.Log($"[NextRound] mainNumber: {mainNumber}, digit: {digit}, currentTotal: {currentTotal}");
        
        UpdateCurrentTotalUI();
        timeBar.ResetBar();
        // Устанавливаем скорость появления пузырьков в зависимости от режима
        bubbleSpawner.pauseBetweenBubbles = modeParams.bubbleSpawnInterval;
        bubbleSpawner.autoDestroyTime = modeParams.autoDestroyTime;
        // Например, 10 пузырей на раунд, диапазон digit как сейчас
        bool excludeOne = (op == "*" || op == "/");
        bubbleSpawner.SpawnBubblesWithOneCorrect(digit, 30, modeParams.digitMin, modeParams.digitMax, excludeOne);
    }
     public void OnTimeBarEnd()
   {
    // Если время истекло и игрок не ответил, начинаем новый раунд
    NextRound();
    }
    public void CheckAnswer(int selectedDigit)
{
    int result = 0;
    switch (operationManager.currentOperation)
    {
        case "+":
            result = currentTotal + selectedDigit;
            break;
        case "-":
            result = currentTotal - selectedDigit;
            break;
        case "*":
            result = currentTotal * selectedDigit;
            break;
        case "/":
            result = currentTotal / selectedDigit;
            break;
    }
    if (result == mainNumberManager.mainNumber)
    {
        int scoreGain = Mathf.RoundToInt(20 * GetCurrentParams().scoreMultiplier);
        score += scoreGain;
        Debug.Log($"Правильный ответ! +{scoreGain} очков. Текущий счет: {score}");
        
        AudioManager.Instance.PlaySuccess();
        UpdateScoreUI();
        gameTimer.ResetTimer();
        rightAnswerAnimation.Play();
        // Переходы между режимами
        if (currentMode == GameModeType.Easy && score >= 250)
            currentMode = GameModeType.Normal;
        else if (currentMode == GameModeType.Normal && score >= 750)
            currentMode = GameModeType.Hard;
        else if (currentMode == GameModeType.Hard && score >= 1500)
            currentMode = GameModeType.Expert;
        else if (currentMode == GameModeType.Expert && score >= 3000)
            currentMode = GameModeType.Master;
        SaveProgress();
        NextRound();
    }
    else
    {
        wrongAnswerAnimation.Play();
        AudioManager.Instance.PlayFail();
        SaveProgress();
        StartCoroutine(EndGameAfterDelay(0.7f));
    }
    
    
}

    public void UpdateCurrentTotalUI()
    {
        if (currentTotalText != null)
            currentTotalText.text = currentTotal.ToString();
    }
    public void UpdateScoreUI()
{
    if (scoreText != null)
        scoreText.text = score.ToString();
}

    void EndGame()
    {
        // Скрыть UI
        gameUI.SetActive(false);
        // Остановить спавн пузырей
        bubbleSpawner.StopSpawning();
        foreach (var bubble in GameObject.FindGameObjectsWithTag("Bubble"))
            Destroy(bubble);
        
        // Сохраняем текущий счет перед сбросом
        int finalScore = score;
        savedScore = finalScore; // Сохраняем счет для возможного продолжения игры
        
        Debug.Log($"EndGame: сохраняем счет {finalScore} в savedScore: {savedScore}");
        
        SubmitScore(finalScore);
        
        
        // Обновляем UI с финальным счетом
        if (yourScoreText != null)
        {
            // Проверяем, есть ли LocalizedTextComponent
            LocalizedTextComponent localizedComponent = yourScoreText.GetComponent<LocalizedTextComponent>();
            if (localizedComponent != null)
            {
                // Обновляем LocalizedTextComponent с правильными параметрами
                localizedComponent.SetFormatArgs(finalScore);
                Debug.Log($"Обновляем LocalizedTextComponent с параметром: {finalScore}");
            }
            else
            {
                // Если нет LocalizedTextComponent, используем прямой доступ
                if (LocalizationManager.Instance != null)
                {
                    string localizedText = LocalizationManager.Instance.GetText("your_score", finalScore);
                    yourScoreText.text = localizedText;
                    Debug.Log($"Обновляем счет напрямую: {localizedText} (финальный счет: {finalScore})");
                }
                else
                {
                    // Fallback если LocalizationManager недоступен
                    yourScoreText.text = $"Ваш счет: {finalScore}";
                    Debug.LogWarning("LocalizationManager недоступен, используем fallback текст");
                }
            }
        }
        else
        {
            Debug.LogError("yourScoreText не назначен в Inspector!");
        }
        
        // Показываем панель лидерборда
        leaderboardPanel.SetActive(true);
        
        // Сохраняем счет в PlayerPrefs перед сбросом для продолжения игры
        PlayerPrefs.SetInt("SavedScoreForContinue", savedScore);
        PlayerPrefs.Save();
        
        Debug.Log($"Сохраняем счет {savedScore} в PlayerPrefs для продолжения игры");
        
        // Сбросить текущий счет ПОСЛЕ обновления UI
        score = 0;
        savedScore = 0; // Сбрасываем сохраненный счет тоже
        SaveProgress(); // чтобы PlayerPrefs тоже сбросился
        
        // Уведомляем менеджер рекламы о завершении игры
        if (rewardedAdManager != null)
        {
            rewardedAdManager.OnGameEnded();
        }
        
        Debug.Log($"Игра окончена! Финальный счет: {finalScore}");
    }

    private GameModeParams GetCurrentParams()
    {
        switch (currentMode)
        {
            case GameModeType.Normal: return gameMode.normalParams;
            case GameModeType.Hard: return gameMode.hardParams;
            case GameModeType.Expert: return gameMode.expertParams;
            case GameModeType.Master: return gameMode.masterParams;
            default: return gameMode.easyParams;
    }
}
private IEnumerator EndGameAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    EndGame();
}
public void SubmitScore(int score)
{
    // Сравнивайте с локальным рекордом, чтобы не затирать лучший результат меньшим
    int bestScore = PlayerPrefs.GetInt("HighScore", 0);
    if (score > bestScore)
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save();
        YG.YG2.SetLeaderboard("TechnoNameLB", score);
    }
}
public void SaveProgress()
{
    PlayerPrefs.SetInt("CurrentScore", score);
    PlayerPrefs.SetInt("HighScore", highScore);
    PlayerPrefs.Save();
}

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CurrentScore");
        PlayerPrefs.DeleteKey("SavedScoreForContinue");
        PlayerPrefs.Save();
        Debug.Log("Прогресс полностью сброшен");
    }
    
    // Метод для сброса только текущего счета (без сохраненного для продолжения)
    public void ResetCurrentScore()
    {
        score = 0;
        savedScore = 0;
        PlayerPrefs.SetInt("CurrentScore", 0);
        PlayerPrefs.Save();
        UpdateScoreUI();
        Debug.Log("Текущий счет сброшен");
    }
    
    // Метод для продолжения игры с сохраненным счетом
    public void ContinueGameWithSavedScore()
    {
        // Загружаем сохраненный счет из PlayerPrefs
        int loadedScore = PlayerPrefs.GetInt("SavedScoreForContinue", 0);
        Debug.Log($"ContinueGameWithSavedScore: загруженный счет из PlayerPrefs = {loadedScore}");
        
        // Восстанавливаем сохраненный счет
        score = loadedScore;
        savedScore = loadedScore; // Обновляем и локальную переменную
        Debug.Log($"Продолжаем игру с восстановленным счетом: {score}");
        
        // Обновляем UI
        UpdateScoreUI();
        
        // Восстанавливаем игровой режим на основе счета
        RestoreGameMode();
        
        // Очищаем сохраненный счет из PlayerPrefs после успешного восстановления
        PlayerPrefs.DeleteKey("SavedScoreForContinue");
        PlayerPrefs.Save();
        Debug.Log("Счет успешно восстановлен, сохраненный счет очищен из PlayerPrefs");
        
        // Начинаем новый раунд
        NextRound();
    }
    
    // Восстанавливаем игровой режим на основе счета
    private void RestoreGameMode()
    {
        if (score >= 3000)
            currentMode = GameModeType.Master;
        else if (score >= 1500)
            currentMode = GameModeType.Expert;
        else if (score >= 750)
            currentMode = GameModeType.Hard;
        else if (score >= 250)
            currentMode = GameModeType.Normal;
        else
            currentMode = GameModeType.Easy;
            
        Debug.Log($"Восстановлен игровой режим: {currentMode} для счета {score}");
    }

void OnApplicationPause(bool pauseStatus)
{
    Debug.Log($"OnApplicationPause: {pauseStatus}, isManuallyPaused: {isManuallyPaused}");
    
    if (!pauseStatus) // Возвращаемся в игру
    {
        // ВСЕГДА выставляем Time.timeScale в зависимости от состояния паузы
        Time.timeScale = isManuallyPaused ? 0f : 1f;
        SaveProgress();
        
        // Обновляем состояние пузырей
        Bubbles.IsPaused = isManuallyPaused;
    }
}

void OnApplicationFocus(bool hasFocus)
{
    Debug.Log($"OnApplicationFocus: {hasFocus}, isManuallyPaused: {isManuallyPaused}");
    
    if (hasFocus) // Возвращаемся в игру
    {
        // ВСЕГДА выставляем Time.timeScale в зависимости от состояния паузы
        Time.timeScale = isManuallyPaused ? 0f : 1f;
        SaveProgress();
        
        // Обновляем состояние пузырей
        Bubbles.IsPaused = isManuallyPaused;
    }
}



}

