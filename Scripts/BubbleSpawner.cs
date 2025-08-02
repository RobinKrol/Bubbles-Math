using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[System.Serializable]
public class ForbiddenZone
{
    public Vector2 center; // Центр зоны
    public Vector2 size;   // Размер зоны (ширина и высота)

    // Проверка, находится ли точка внутри зоны
    public bool Contains(Vector2 point)
    {
        Vector2 min = center - size / 2f;
        Vector2 max = center + size / 2f;
        return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
    }
}

public class BubbleSpawner : MonoBehaviour
{
    [Header("Префаб пузырька")]
    public GameObject bubblePrefab;

    [Header("Область спавна")]
    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;
   
    [Header("Запрещённые зоны")]
    public List<ForbiddenZone> forbiddenZones;

    [Header("Параметры спавна")]
    public float pauseBetweenBubbles = 1f;
    public float autoDestroyTime = 3f;

    [Header("Managers")] 
    public GameMode gameMode;

    private List<Bubbles> activeBubbles = new List<Bubbles>();
    private float timer = 0f;
    private bool isSpawning = true;
    private Coroutine spawnCoroutine;



    void Awake()
    {
      CalculateSpawnArea();
    }

    void Update()
    {
        if (!isSpawning) return;
        
        // Проверяем, не стоит ли игра на паузе
        if (Bubbles.IsPaused || Time.timeScale == 0f)
        {
            return; // Не спавним пузыри во время паузы
        }

        timer += Time.deltaTime;
        if (timer >= pauseBetweenBubbles)
        {
            timer = 0f;
         
        }
    }
    private void CalculateSpawnArea()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector3 camCenter = cam.transform.position;

        spawnAreaMin = new Vector2(camCenter.x - camWidth / 2f, camCenter.y - camHeight / 2f);
        spawnAreaMax = new Vector2(camCenter.x + camWidth / 2f, camCenter.y + camHeight / 2f);
    }
    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    public void SpawnBubblesWithOneCorrect(int correctDigit, int bubblesCount, int minDigit, int maxDigit, bool excludeOne = false)
    {
        // Проверяем, не стоит ли игра на паузе
        if (Bubbles.IsPaused || Time.timeScale == 0f)
        {
            Debug.Log("SpawnBubblesWithOneCorrect: Game is paused, skipping spawn");
            return;
        }
        
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        List<int> digits = new List<int>(bubblesCount) { correctDigit };
        for (int i = 1; i < bubblesCount; i++)
    {
            int value;
            do {
                value = Random.Range(minDigit, maxDigit + 1);
            } while (excludeOne && value == 1);
            digits.Add(value);
    }
    // Перемешиваем
    for (int i = 0; i < digits.Count; i++)
    {
        int j = Random.Range(i, digits.Count);
        (digits[i], digits[j]) = (digits[j], digits[i]); // Swap
        }
        spawnCoroutine = StartCoroutine(SpawnBubblesWithDelay(digits));
    }
    private IEnumerator SpawnBubblesWithDelay(List<int> digits)
{
        foreach (int digit in digits)
        {
            // Проверяем паузу перед каждым спавном
            if (Bubbles.IsPaused || Time.timeScale == 0f)
            {
                Debug.Log("SpawnBubblesWithDelay: Game is paused, stopping spawn");
                yield break; // Прерываем корутину
            }
            
            SpawnBubble(digit);
            yield return new WaitForSeconds(pauseBetweenBubbles); // пауза между пузырями
        }
    }
    void SpawnBubble(int digit)
    {
        // Аварийный выход, если область спавна не вычислена
        if (spawnAreaMin == Vector2.zero && spawnAreaMax == Vector2.zero) 
        {
        Debug.LogError("Spawn area not calculated!");
        return;
        }

        Vector2 spawnPos = (spawnAreaMin + spawnAreaMax) / 2f; // Центр области;
        int attempts = 0;
        bool positionFound = false;
        while (attempts < 100) {
            spawnPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );
            positionFound = !IsInForbiddenZone(spawnPos) && !IsOverlappingOtherBubbles(spawnPos);
            if (positionFound) 
            {
            positionFound = !IsOverlappingOtherBubbles(spawnPos);
            }

        if (positionFound) break;
        attempts++;
            
        }
        
        if (!positionFound)
        {
            Debug.LogWarning("Не удалось найти идеальное место. Пузырь появится в случайной позиции.");
        spawnPos = new Vector2
        (
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );
        }

        GameObject bubbleObj = Instantiate(bubblePrefab, new Vector3(spawnPos.x, spawnPos.y, 0), 
        Quaternion.identity, transform);

        // Получаем компонент Bubble у созданного объекта
        if (bubbleObj.TryGetComponent(out Bubbles bubble)) {
            bubble.SetValue(digit);
            bubble.SetAutoDestroyTime(autoDestroyTime);
            activeBubbles.Add(bubble);
        }
    } 

     bool IsInForbiddenZone(Vector2 pos)
    {
        foreach (var zone in forbiddenZones)
        {
            if (zone.Contains(pos))
                return true;
        }
        return false;
    }

    bool IsOverlappingOtherBubbles(Vector2 pos)
    {
        const float minDistance = 1.5f; // Минимальное расстояние между пузырями
        activeBubbles.RemoveAll(b => b == null || b.gameObject == null);

        foreach (var bubble in activeBubbles)
        {
            if (Vector2.Distance(pos, bubble.transform.position) < minDistance)
                return true;
        }
        return false;
    }

    void OnDrawGizmos()
{
    Gizmos.color = new Color(1, 0, 0, 0.3f);

    if (forbiddenZones != null)
    {
        foreach (var zone in forbiddenZones)
        {
            Vector3 center = new Vector3(zone.center.x, zone.center.y, 0);
            Vector3 size = new Vector3(zone.size.x, zone.size.y, 0.1f);
            Gizmos.DrawCube(center, size);
        }

            // Визуализация области спавна (зелёная рамка)
            Gizmos.color = Color.green;
            Vector3 spawnAreaCenter = (spawnAreaMin + spawnAreaMax) / 2f;
            Vector3 spawnAreaSize = new Vector3(
                spawnAreaMax.x - spawnAreaMin.x,
                spawnAreaMax.y - spawnAreaMin.y,
                0.1f
            );
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
}


}

