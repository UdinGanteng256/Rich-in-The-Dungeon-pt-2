using UnityEngine;

[System.Serializable]
public class OreSpawnData
{
    public string name; 
    public GameObject prefab;
    [Range(0, 100)] public float chance; 
}

public class RandomizeOre : MonoBehaviour
{
    [Header("Setting Batu & Persentase")]
    public OreSpawnData[] oreList; 

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
            Debug.LogError("Mana Collidernya?");
            return;
        }

        if (oreList == null || oreList.Length == 0) return;

        int count = 0;
        int maxAttempts = 100; 

        while (count < totalRocksToSpawn && maxAttempts > 0)
        {
            maxAttempts--;

            Vector2 randomPos = GetRandomPositionInBox();

            if (!Physics2D.OverlapCircle(randomPos, 0.5f, obstacleLayer))
            {
                // prefab berdasarkan persentase
                GameObject selectedPrefab = GetRandomWeightedPrefab();

                if (selectedPrefab != null)
                {
                    Instantiate(selectedPrefab, randomPos, Quaternion.identity);
                    count++;
                }
            }
        }
    }

    GameObject GetRandomWeightedPrefab()
    {
        float totalWeight = 0f;
        foreach (var ore in oreList)
        {
            totalWeight += ore.chance;
        }

        float randomPoint = Random.Range(0, totalWeight);

        foreach (var ore in oreList)
        {
            if (randomPoint < ore.chance)
            {
                return ore.prefab;
            }
            else
            {
                randomPoint -= ore.chance;
            }
        }

        return oreList[oreList.Length - 1].prefab;
    }

    Vector2 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
}