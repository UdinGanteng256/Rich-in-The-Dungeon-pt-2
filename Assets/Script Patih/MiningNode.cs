using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // WAJIB ADA

public class MiningNode : MonoBehaviour
{
    [Header("Statistik Batu")]
    public int durability = 3; 
    public GameObject lootPrefab; 

    [Header("Visual Feedback")]
    public float shakeAmount = 0.1f; 
    public SpriteRenderer spriteRenderer;

    private int currentHealth;
    private Vector3 originalPos;
    private bool isShaking = false;
    private Camera mainCamera; // Tambahan untuk Raycast

    void Start()
    {
        currentHealth = durability;
        originalPos = transform.position;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Cache kamera utama biar ringan
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Deteksi Klik Kiri (Manual via Script)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckHit();
        }
    }

    void CheckHit()
    {
        // 1. Ubah posisi mouse di layar ke dunia game
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        // 2. Tembakkan Laser
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // 3. Cek apakah laser kena SAYA (Batu ini)?
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        // Logika Pukul Batu (Sama seperti sebelumnya)
        currentHealth--;

        if (!isShaking) StartCoroutine(ShakeEffect());

        Debug.Log($"Batu dipukul! Sisa nyawa: {currentHealth}");

        if (currentHealth <= 0)
        {
            BreakRock();
        }
    }

    void BreakRock()
    {
        if (lootPrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3.up * 0.5f); 
            Instantiate(lootPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("Batu Hancur!");
        Destroy(gameObject);
    }

    IEnumerator ShakeEffect()
    {
        isShaking = true;
        float elapsed = 0.0f;
        float duration = 0.1f;

        spriteRenderer.color = new Color(1f, 0.8f, 0.8f); 

        while (elapsed < duration)
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