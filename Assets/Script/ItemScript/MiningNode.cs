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

    [Header("Visual & VFX")]
    public float shakeAmount = 0.1f;
    public SpriteRenderer spriteRenderer;
    
    [Tooltip("Effect pas batu dipukul (Debu/Percikan)")]
    public GameObject hitVFX; 
    
    [Tooltip("Effect pas batu hancur lebur (Ledakan batu)")]
    public GameObject breakVFX;

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

        // 1. Mainkan Effect Hit (Debu pukulan)
        if (hitVFX != null)
        {
            Instantiate(hitVFX, transform.position, Quaternion.identity);
        }

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
        // 2. Mainkan Effect Hancur (Pecahan batu)
        if (breakVFX != null)
        {
            Instantiate(breakVFX, transform.position, Quaternion.identity);
        }

        if (lootPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);
            
            LootObject lootScript = loot.GetComponent<LootObject>();
            if (lootScript != null)
            {
                lootScript.useRandomSize = randomSizeOnDrop;
            }
            
            Rigidbody2D rb = loot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(1f, 3f)
                );
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    IEnumerator ShakeEffect()
    {
        isShaking = true;
        float elapsed = 0f;
        
        // Simpan warna asli (jaga-jaga kalau batunya gak putih)
        Color originalColor = spriteRenderer.color;
        
        // Efek Flash Merah
        spriteRenderer.color = new Color(1f, 0.7f, 0.7f); 

        while (elapsed < 0.1f)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        spriteRenderer.color = originalColor; // Balikin warna semula
        isShaking = false;
    }
}