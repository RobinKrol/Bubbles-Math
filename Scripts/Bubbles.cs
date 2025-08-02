using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Bubbles : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public int digitValue;
    public static bool IsPaused = false;
    private float autoDestroyTime = 3f; // значение по умолчанию
    
     void OnEnable()
    {
        StartCoroutine(Appear());
        StartCoroutine(AutoDestroy());
    }

    public void OnBubbleClick()
    {
    if (IsPaused)
        return;
    GameModeManager manager = FindAnyObjectByType<GameModeManager>();
    if (manager != null)
    {
        manager.CheckAnswer(digitValue);
    }
    AudioManager.Instance.PlayClick();
    
    // Вызов вибрации с улучшенной системой
    if (VibrationHelper.IsVibrationSupported())
    {
        VibrationHelper.VibrateOnce();
    }
    
    DestroySelf(); // пузырь исчезает после клика
    }
    public void SetValue(int value)
    {
       digitValue = value;
        if (textMesh != null)
            {textMesh.text = value.ToString();
            Debug.Log("Устанавливаю значение: " + value);
            }
    }
    public void SetAutoDestroyTime(float time)
  {
      autoDestroyTime = time;
  }

     private System.Collections.IEnumerator Appear()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        transform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = t;
            yield return null;
        }

        transform.localScale = endScale;
        canvasGroup.alpha = 1f;
    }
     private System.Collections.IEnumerator PopAndDestroy()
    {
        float duration = 0.15f; // Длительность анимации
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        /// Получаем CanvasGroup
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Уменьшаем размер
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            // Плавно уменьшаем прозрачность
            canvasGroup.alpha = 1 - t;
            yield return null;
        }

        Destroy(gameObject);
    }
    private System.Collections.IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(autoDestroyTime);
      StartCoroutine(PopAndDestroy());
    }
      public void DestroySelf()
    {
        StartCoroutine(PopAndDestroy());
    }
}
