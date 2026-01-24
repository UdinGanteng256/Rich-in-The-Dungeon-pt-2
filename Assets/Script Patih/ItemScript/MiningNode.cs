using UnityEngine;
using System.Collections;

public class MiningNode : MonoBehaviour
{
    [Header("Statistik Batu")]
    public int durability = 3;
    public GameObject lootPrefab;

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

    // Dipanggil oleh PlayerAction saat pemain menambang
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

    // Hancurkan batu dan keluarkan loot
    void BreakRock()
    {
        if (lootPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(lootPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // Efek goyang & flash saat batu dipukul
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
