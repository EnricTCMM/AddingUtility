using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class StatBar : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How fast the bar fills or empties. Higher is faster.")]
    public float fillSpeed = 5f;

    [Header("UI References")]
    [Tooltip("The text object that shows the name of the stat (e.g., HUNGER)")]
    public TextMeshProUGUI labelText;
    
    [Tooltip("The text object that shows the numbers (e.g., 80 / 100)")]
    public TextMeshProUGUI valueText;

    private Slider slider;
    private float targetValue;
    private float currentMaxValue;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (slider != null && !Mathf.Approximately(slider.value, targetValue))
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * fillSpeed);
            UpdateValueText(slider.value, currentMaxValue);
        }
    }

    // Now it receives the statName as well
    public void InitializeBar(string statName, float currentValue, float maxValue)
    {
        if (slider == null) return;

        // Set the label text automatically
        if (labelText != null)
        {
            labelText.text = statName;
        }

        slider.maxValue = maxValue;
        slider.value = currentValue; 
        targetValue = currentValue;
        currentMaxValue = maxValue;
        
        UpdateValueText(currentValue, maxValue);
    }

    public void UpdateBar(float currentValue, float maxValue)
    {
        targetValue = currentValue;
        currentMaxValue = maxValue;
    }

    private void UpdateValueText(float currentValue, float maxValue)
    {
        if (valueText != null)
        {
            valueText.text = $"{Mathf.RoundToInt(currentValue)} / {Mathf.RoundToInt(maxValue)}";
        }
    }
}