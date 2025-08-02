using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameTimerController : MonoBehaviour
{
    public Image timerCircle;
    public TextMeshProUGUI timerText;
    public float totalTime = 60f;

    private float timer;
    public Action OnGameTimerEnd;
    void Start()
    {
        timer = totalTime;
        UpdateUI();
    }

    
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                UpdateUI();
                OnGameTimerEnd?.Invoke(); 
            }
            else
            {
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timer).ToString();

        
        if (timerCircle != null)
            timerCircle.fillAmount = timer / totalTime;
    }
     public void ResetTimer()
   {
       timer = totalTime;
       UpdateUI();
   }
}
