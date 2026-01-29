using UnityEngine;
using UnityEngine.EventSystems; // Wajib ada buat deteksi UI

public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxUIHover);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.sfxButtonClick);
        }
    }
}