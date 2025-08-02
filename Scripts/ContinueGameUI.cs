using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContinueGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public RectTransform panelRect;
    public Button continueButton;
    public Button closeButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI descriptionText;
    
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public float scaleMultiplier = 1.1f;
    
    private Vector3 originalScale;
    private bool isAnimating = false;
    
    void Awake()
    {
        // Получаем компоненты если они не назначены
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
            
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();
            
        // Сохраняем оригинальный размер
        originalScale = panelRect.localScale;
        
        // Настраиваем начальное состояние
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        // Скрываем панель
        gameObject.SetActive(false);
    }
    
    public void ShowPanel()
    {
        if (isAnimating) return;
        
        // Активируем объект если он неактивен
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
            
        StartCoroutine(ShowAnimation());
    }
    
    public void HidePanel()
    {
        if (isAnimating) return;
        
        // Если объект неактивен, просто скрываем его без анимации
        if (!gameObject.activeInHierarchy)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            return;
        }
        
        StartCoroutine(HideAnimation());
    }
    
    private System.Collections.IEnumerator ShowAnimation()
    {
        isAnimating = true;
        
        // Начальное состояние
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        panelRect.localScale = originalScale * scaleMultiplier;
        
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Плавное появление
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                
            // Плавное масштабирование
            panelRect.localScale = Vector3.Lerp(originalScale * scaleMultiplier, originalScale, t);
            
            yield return null;
        }
        
        // Финальное состояние
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
            
        panelRect.localScale = originalScale;
        isAnimating = false;
    }
    
    private System.Collections.IEnumerator HideAnimation()
    {
        isAnimating = true;
        
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Плавное исчезновение
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                
            // Плавное масштабирование
            panelRect.localScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, t);
            
            yield return null;
        }
        
        // Финальное состояние
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        gameObject.SetActive(false);
        isAnimating = false;
    }
    public void UpdateScore(int score)
{
    if (scoreText != null)
         scoreText.text = LocalizationManager.Instance.GetText("your_score", score);
}
    
    public void SetButtonInteractable(bool interactable)
    {
        if (continueButton != null)
            continueButton.interactable = interactable;
    }
    
    public void UpdateTexts(string title, string description)
    {
        Debug.Log($"UpdateTexts called: title={title}, description={description}");
        if (titleText != null)
            titleText.text = title;
            
        if (descriptionText != null)
            descriptionText.text = description;
    }
} 