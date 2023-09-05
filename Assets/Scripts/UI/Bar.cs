using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] protected Slider Slider;

    public void OnValueChanged(float value, float maxValue)
    {
        Slider.value = value / maxValue;
    }
}
