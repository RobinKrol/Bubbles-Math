using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject[] steps;
    private int currentStep = 0;
    private bool tutorialActive = false;
    
    void Start()
    {
        tutorialPanel.SetActive(false);
        foreach (var step in steps)
            step.SetActive(false);
            
        // Подписываемся на события локализации
        LocalizationManager.OnLanguageChanged += OnLanguageChanged;
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
    }
    
    private void OnLanguageChanged(string newLanguage)
    {
        // Если туториал активен, обновляем тексты
        if (tutorialActive)
        {
            UpdateTutorialTexts();
        }
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        currentStep = 0;
        tutorialActive = true;
        ShowStep(currentStep);
        
        // Обновляем локализацию для туториала
        UpdateTutorialTexts();
    }
    
    public void NextStep()
    {
        steps[currentStep].SetActive(false);
        currentStep++;
        if (currentStep < steps.Length)
        {
            ShowStep(currentStep);
            // Обновляем локализацию для нового шага
            UpdateTutorialTexts();
        }
        else
        {
            CloseTutorial();
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = false;
        foreach (var step in steps)
            step.SetActive(false);
    }

    private void ShowStep(int stepIndex)
    {
        for (int i = 0; i < steps.Length; i++)
            steps[i].SetActive(i == stepIndex);
    }
    
    private void UpdateTutorialTexts()
    {
        // Обновляем локализацию в панели туториала
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.UpdateTextsInGameObject(tutorialPanel);
        }
    }

    void Update()
    {
        if (tutorialActive && Mouse.current.leftButton.wasPressedThisFrame)
        {
            NextStep();
        }
    }
}
