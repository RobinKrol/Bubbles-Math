using UnityEngine;
using TMPro;
public class AnswerTextAnimation : MonoBehaviour
{
    public GameModeManager gameModeManager;
    public TextMeshProUGUI text;
    public float showDuration = 0.01f;
    public float fadeDuration = 0.01f;
    public float scaleUp = 1.3f;

    private Vector3 originalScale;
    private Coroutine animCoroutine;

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        originalScale = transform.localScale;
        text.alpha = 0f;
        
    }
    public void Play()
    {
        Debug.Log($"[AnswerTextAnimation] Play() called on {gameObject.name}");
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        text.alpha = 1f;
        animCoroutine = StartCoroutine(Animate());
    }
    
    public void ResetAnimation()
    {
        // Останавливаем текущую анимацию
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        
        // Сбрасываем состояние текста
        if (text != null)
        {
            text.alpha = 0f;
        }
        
        // Сбрасываем масштаб
        transform.localScale = originalScale;

    }
    private System.Collections.IEnumerator Animate()
    {
        
        text.alpha = 0f;
        transform.localScale = originalScale * scaleUp;

        // Fade in + scale down to normal
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = t / fadeDuration;
            text.alpha = Mathf.Lerp(0f, 1f, k);
            transform.localScale = Vector3.Lerp(originalScale * scaleUp, originalScale, k);
            yield return null;
        }
        text.alpha = 1f;
        transform.localScale = originalScale;

        // Hold
        yield return new WaitForSeconds(showDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = t / fadeDuration;
            text.alpha = Mathf.Lerp(1f, 0f, k);
            yield return null;
        }
        text.alpha = 0f;
    }

    
}
