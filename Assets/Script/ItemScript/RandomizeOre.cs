using UnityEngine;

public class RandomizeOre : MonoBehaviour
{
    [Header("Setting Batu")]
    public GameObject[] rockPrefabs; 

    [Tooltip("Berapa banyak batu yang mau dimunculkan?")]
    public int totalRocksToSpawn = 5;

    [Header("Area Batu Spawn")]
    public BoxCollider2D spawnArea; 

    [Header("Validasi Posisi")]
    public LayerMask obstacleLayer; 

    void Start()
    {
        SpawnRocks();
    }

    void SpawnRocks()
    {
        if (spawnArea == null)
        {
            Debug.LogError("Lupa masukin BoxCollider2D untuk area spawn!");
            return;
        }

        int count = 0;
        int maxAttempts = 100; 

        while (count < totalRocksToSpawn && maxAttempts > 0)
        {
            maxAttempts--;

            Vector2 randomPos = GetRandomPositionInBox();

            if (!Physics2D.OverlapCircle(randomPos, 0.5f, obstacleLayer))
            {
                int randomIndex = Random.Range(0, rockPrefabs.Length);
                GameObject selectedPrefab = rockPrefabs[randomIndex];

                Instantiate(selectedPrefab, randomPos, Quaternion.identity);
                count++;
            }
        }
    }

    Vector2 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
}