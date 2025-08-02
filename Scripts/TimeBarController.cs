using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeBarController : MonoBehaviour
{
    public Image barFill;
    public float roundTime = 30f; 
    private float timer;
    private RectTransform barRect;
    private float fullWidth;

 // Событие окончания времени
    public Action OnRoundEnd;
    void Start()
    {
        barRect = barFill.rectTransform;
        fullWidth = barRect.sizeDelta.x;
        timer = roundTime;
        ResetBar();
    }

    
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            float t = Mathf.Clamp01(timer / roundTime);
            float newWidth = fullWidth * t;
            barRect.sizeDelta = new Vector2(newWidth, barRect.sizeDelta.y);

           if (timer <= 0)
        {
            timer = 0;
            OnRoundEnd?.Invoke();
        }
        }
    }
    public void ResetBar()
    {
        // Если barRect ещё не инициализирован, инициализируем его здесь
    if (barRect == null && barFill != null)
    {
        barRect = barFill.rectTransform;
        fullWidth = barRect.sizeDelta.x;
    }
    if (barRect == null)
    {
        Debug.LogError("barRect не инициализирован! Проверьте, что barFill назначен в инспекторе.");
        return;
    }
    timer = roundTime;
    barRect.sizeDelta = new Vector2(fullWidth, barRect.sizeDelta.y);
    }
}
