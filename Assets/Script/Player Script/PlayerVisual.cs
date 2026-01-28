using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer handRenderer;

    [SerializeField] public Animator animator;

    public void UpdateHandSprite(ItemData item)
    {
        if (handRenderer == null) 
        {
            Debug.LogWarning("Slot blm diisi");
            return;
        }

        if (item == null)
        {
            handRenderer.sprite = null;
            handRenderer.enabled = false; 
            return;
        }

        handRenderer.sprite = item.icon;
        handRenderer.enabled = true;
    }
}