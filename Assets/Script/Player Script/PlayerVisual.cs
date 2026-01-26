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

    public void PlayMiningAnimation()
    {
        StartCoroutine(SimpleSwingEffect());
    }

    System.Collections.IEnumerator SimpleSwingEffect()
    {
        animator.SetBool("isMining", true);
        
        Quaternion startRotation =
            handRenderer.transform.localRotation;

        handRenderer.transform.localRotation =
            Quaternion.Euler(0, 0, -90);

        yield return new WaitForSeconds(0.1f);

        handRenderer.transform.localRotation = startRotation;
        animator.SetBool("isMining", false);
    }
}
