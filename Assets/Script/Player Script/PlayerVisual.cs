using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer handRenderer;

    // Animasi
    [SerializeField] public Animator animator;

    public void UpdateHandSprite(ItemData item)
    {
        if (item == null)
        {
            handRenderer.sprite = null;
            handRenderer.enabled = false;
            return;
        }

        handRenderer.sprite = item.icon;
        handRenderer.enabled = true;

        if (handRenderer == null) return;

    }
}
