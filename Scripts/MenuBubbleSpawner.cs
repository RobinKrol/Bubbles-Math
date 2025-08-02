using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class MenuBubbleSpawner : MonoBehaviour
{
    public GameObject menuBubblePrefab;
    private float autoDestroyTime = 3f;
    public float pauseBetweenBubbles = 1f;
    public static MenuBubbleSpawner Instance;
    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;

    public List<ForbiddenZone> forbiddenZones;
    private List<MenuBubbles> activeBubbles = new List<MenuBubbles>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CalculateSpawnArea();
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


    void Start()
    {
        StartCoroutine(SpawnBubblesLoop());
    }

    private IEnumerator SpawnBubblesLoop()
    {
        while (true)
        {
            SpawnBubble();
            yield return new WaitForSeconds(pauseBetweenBubbles);
        }
    }

    void SpawnBubble()
    {
        
        if (spawnAreaMin == Vector2.zero && spawnAreaMax == Vector2.zero)
        {
            Debug.LogError("Spawn area not calculated!");
            return;
        }

        Vector2 spawnPos = Vector2.zero;
        int attempts = 0;
        bool positionFound = false;
        while (attempts < 100)
        {
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
            spawnPos = new Vector2
            (
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
        }

        GameObject bubbleObj = Instantiate(menuBubblePrefab, new Vector3(spawnPos.x, spawnPos.y, 0),
        Quaternion.identity, transform);

        float scale = Random.Range(0.5f, 1.5f);
        RectTransform rt = bubbleObj.GetComponent<RectTransform>();
        if (rt != null)
        rt.localScale = new Vector3(scale, scale, 1f);

        MenuBubbles bubble = bubbleObj.GetComponent<MenuBubbles>();
        if (bubble != null)
        {
            bubble.autoDestroyTime = autoDestroyTime;
            activeBubbles.Add(bubble);
        }
    }

    public void RemoveBubble(MenuBubbles bubble)
    {
        activeBubbles.Remove(bubble);
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
        const float minDistance = 1.5f; 
        activeBubbles.RemoveAll(b => b == null || b.gameObject == null);

        foreach (var bubble in activeBubbles)
        {
            if (bubble != null && bubble.gameObject != null)
        {
            if (Vector2.Distance(pos, bubble.transform.position) < minDistance)
                return true;
        }
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
