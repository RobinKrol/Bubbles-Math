using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;
using System;

public class RewardedAdManager : MonoBehaviour
{
    [Header("UI Elements")]
    public ContinueGameUI continueGameUI;
    public Button continueGameButton;
    public TextMeshProUGUI continueGameText;
    public Button closeButton;
    
    [Header("Settings")]
    public string rewardID = "continue_game";
    public float panelShowDelay = 1f;
    
    [Header("References")]
    public GameModeManager gameModeManager;
    
    private bool isAdAvailable = false;
    private bool isAdShowing = false;
    
    void Start()
    {
        // Подписываемся на события рекламы
        YG2.onRewardAdv += OnRewardReceived;
        YG2.onOpenRewardedAdv += OnAdOpened;
        YG2.onCloseRewardedAdv += OnAdClosed;
        YG2.onErrorRewardedAdv += OnAdError;
        
        // Настраиваем UI
        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(ShowRewardedAd);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseContinuePanel);
            
        // Убеждаемся, что панель скрыта в начале
        if (continueGameUI != null && continueGameUI.gameObject.activeInHierarchy)
            continueGameUI.HidePanel();
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        YG2.onRewardAdv -= OnRewardReceived;
        YG2.onOpenRewardedAdv -= OnAdOpened;
        YG2.onCloseRewardedAdv -= OnAdClosed;
        YG2.onErrorRewardedAdv -= OnAdError;
    }
    
    // Вызывается когда игра заканчивается
    public void OnGameEnded()
    {
        // Проверяем доступность рекламы
        CheckAdAvailability();
        
        // Получаем счет из PlayerPrefs, так как он может быть уже сброшен в GameModeManager
        int displayScore = PlayerPrefs.GetInt("SavedScoreForContinue", 0);
        continueGameUI.UpdateScore(displayScore);
        
        Debug.Log($"Игра завершена, счет для продолжения из PlayerPrefs: {displayScore}");
        
        // Показываем панель продолжения игры с задержкой
        Invoke(nameof(ShowContinuePanel), panelShowDelay);
    }
    
    private void CheckAdAvailability()
    {
        // В реальном проекте здесь можно добавить дополнительную логику
        // для проверки доступности рекламы
        isAdAvailable = true;
    }
    
    private void ShowContinuePanel()
    {
        if (continueGameUI != null && isAdAvailable)
        {
            // Обновляем тексты с использованием локализации
            string title = LocalizationManager.Instance.GetText("continue_game_title");
            string description = LocalizationManager.Instance.GetText("continue_game_description");
            continueGameUI.UpdateTexts(title, description);
            
            // Показываем панель с анимацией
            continueGameUI.ShowPanel();
            
            // Обновляем текст кнопки
            if (continueGameText != null)
                continueGameText.text = LocalizationManager.Instance.GetText("continue_game_button");
                
            // Активируем кнопку
            if (continueGameButton != null)
                continueGameButton.interactable = true;
        }
    }
    
    public void CloseContinuePanel()
    {
        if (continueGameUI != null)
            continueGameUI.HidePanel();
            
        // Если игрок закрыл панель без продолжения, очищаем сохраненный счет
        PlayerPrefs.DeleteKey("SavedScoreForContinue");
        PlayerPrefs.Save();
        Debug.Log("Панель закрыта игроком, сохраненный счет очищен");
    }
    
    // Вызывается при нажатии кнопки "Продолжить игру"
    public void ShowRewardedAd()
    {
        if (isAdShowing || !isAdAvailable)
            return;
            
        Debug.Log("Показываем рекламу за вознаграждение...");
        
        // Деактивируем кнопку во время показа рекламы
        if (continueGameButton != null)
            continueGameButton.interactable = false;
            
        if (continueGameUI != null)
            continueGameUI.SetButtonInteractable(false);
            
        // Показываем рекламу
        YG2.RewardedAdvShow(rewardID, () =>
        {
            Debug.Log("Реклама показана, ожидаем вознаграждение...");
        });
    }
    
    // Событие получения вознаграждения
    private void OnRewardReceived(string id)
    {
        Debug.Log($"Получено вознаграждение с ID: {id}");
        
        if (id == rewardID)
        {
            // Продолжаем игру
            ContinueGame();
        }
    }
    
    // Событие открытия рекламы
    private void OnAdOpened()
    {
        Debug.Log("Реклама открыта");
        isAdShowing = true;
    }
    
    // Событие закрытия рекламы
    private void OnAdClosed()
    {
        Debug.Log("Реклама закрыта");
        isAdShowing = false;
        
        // НЕ скрываем панель здесь, так как это может быть часть процесса продолжения игры
        // Панель будет скрыта в ContinueGame() после успешного получения вознаграждения
    }
    
    // Событие ошибки рекламы
    private void OnAdError()
    {
        Debug.LogWarning("Ошибка при показе рекламы");
        isAdShowing = false;
        
        // Активируем кнопку обратно
        if (continueGameButton != null)
            continueGameButton.interactable = true;
            
        if (continueGameUI != null)
            continueGameUI.SetButtonInteractable(true);
    }
    
    // Продолжение игры
    private void ContinueGame()
    {
        Debug.Log("Продолжаем игру после просмотра рекламы");
        
        // Скрываем панель БЕЗ очистки счета
        if (continueGameUI != null)
            continueGameUI.HidePanel();
        
        // Скрываем панель лидерборда если она открыта
        if (gameModeManager != null && gameModeManager.leaderboardPanel != null)
            gameModeManager.leaderboardPanel.SetActive(false);
        
        // Восстанавливаем игровой UI
        if (gameModeManager != null && gameModeManager.gameUI != null)
            gameModeManager.gameUI.SetActive(true);
        
        // Сбрасываем анимации текстов ответов
        if (gameModeManager != null)
        {
            if (gameModeManager.rightAnswerAnimation != null)
            {
                gameModeManager.rightAnswerAnimation.ResetAnimation();
            }
            if (gameModeManager.wrongAnswerAnimation != null)
            {
                gameModeManager.wrongAnswerAnimation.ResetAnimation();
            }
        }
        
        // Сбрасываем таймер игры
        if (gameModeManager != null && gameModeManager.gameTimer != null)
            gameModeManager.gameTimer.ResetTimer();
        
        // Возобновляем игру с сохраненным счетом
        if (gameModeManager != null)
        {
            gameModeManager.ContinueGameWithSavedScore();
        }
        
        // Очищаем все существующие пузырьки
        foreach (var bubble in GameObject.FindGameObjectsWithTag("Bubble"))
            Destroy(bubble);
    }
} 