using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBubbles : MonoBehaviour
{
    
    public float autoDestroyTime = 3f;
    


    void OnEnable()
    {
        StartCoroutine(Appear());
        StartCoroutine(AutoDestroy());
    }

    public void OnBubbleClick()
    {
        MenuAudioManager.Instance.PlayClick();
        DestroySelf();
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
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            canvasGroup.alpha = 1 - t;
            yield return null;
        }

        Destroy(gameObject);
    }
    private System.Collections.IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(autoDestroyTime);
        if (this != null)
            StartCoroutine(PopAndDestroy());
    }

    public void DestroySelf()
    {
        if (MenuBubbleSpawner.Instance != null)
        {
            MenuBubbleSpawner.Instance.RemoveBubble(this);
        }
        StartCoroutine(PopAndDestroy());
    }

}
