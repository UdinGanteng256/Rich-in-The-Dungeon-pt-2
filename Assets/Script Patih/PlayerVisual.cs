using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer handRenderer;

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
        Quaternion startRotation =
            handRenderer.transform.localRotation;

        handRenderer.transform.localRotation =
            Quaternion.Euler(0, 0, -90);

        yield return new WaitForSeconds(0.1f);

        handRenderer.transform.localRotation = startRotation;
    }
}
