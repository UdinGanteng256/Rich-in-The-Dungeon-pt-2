using UnityEngine;
using System.Collections;

public class MiningNode : MonoBehaviour
{
    [Header("Statistik Batu")]
    public int durability = 3;
    public GameObject lootPrefab;
    
    [Header("Drop Settings")]
    [Tooltip("Resource akan drop SATU item dengan size random")]
    public bool randomSizeOnDrop = true;

    [Header("Visual")]
    public float shakeAmount = 0.1f;
    public SpriteRenderer spriteRenderer;

    private int currentHealth;
    private Vector3 originalPos;
    private bool isShaking = false;

    private void Start()
    {
        currentHealth = durability;
        originalPos = transform.position;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage()
    {
        currentHealth--;

        if (!isShaking)
            StartCoroutine(ShakeEffect());

        Debug.Log($"Batu dipukul! HP tersisa: {currentHealth}");

        if (currentHealth <= 0)
        {
            BreakRock();
        }
    }

    void BreakRock()
    {
        if (lootPrefab != null)
        {
            // DROP HANYA 1 RESOURCE dengan size random
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);
            
            // Set agar loot menggunakan random size
            LootObject lootScript = loot.GetComponent<LootObject>();
            if (lootScript != null)
            {
                lootScript.useRandomSize = randomSizeOnDrop;
            }
            
            // Tambahkan efek fisik (opsional)
            Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(1f, 3f)
                );
                rb.AddForce(force, ForceMode2D.Impulse);
            }
            
            Debug.Log("Rock destroyed! Dropped 1 resource with random size.");
        }

        Destroy(gameObject);
    }

    IEnumerator ShakeEffect()
    {
        isShaking = true;

        float elapsed = 0f;
        spriteRenderer.color = new Color(1f, 0.8f, 0.8f);

        while (elapsed < 0.1f)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        spriteRenderer.color = Color.white;
        isShaking = false;
    }
}