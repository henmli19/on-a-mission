using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private RobotHealth robotHealth;
    [SerializeField] private Slider healthSlider;

    void Start()
    {
        // Set max value
        healthSlider.maxValue = robotHealth.MaxHealth;
        healthSlider.value = robotHealth.GetHealth();
    }

    void Update()
    {
        // Update the bar every frame
        healthSlider.value = robotHealth.GetHealth();
    }
}
