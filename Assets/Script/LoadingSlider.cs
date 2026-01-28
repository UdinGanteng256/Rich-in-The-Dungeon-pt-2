using UnityEngine;
using TMPro;

public class LoadingSlider : MonoBehaviour
{
    [SerializeField] private float maxSliderAmount = 100.0f;

    public void SliderChange(float value)
    {
        float localValue = value * maxSliderAmount;
    }
}
