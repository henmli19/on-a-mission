using UnityEngine;
using UnityEngine.UI;

public class BatteryHealthUI : MonoBehaviour
{
    public Image[] bars;   
    public int maxHealth = 6;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].enabled = (i < currentHealth);
        }
    }
}
