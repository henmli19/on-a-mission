using UnityEngine;
using UnityEngine.UI;

public class BatteryHealthUI : MonoBehaviour
{
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;
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
            audioSource.PlayOneShot(deathSound);
        }
        audioSource.PlayOneShot(damageSound);
        
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
    
        // Clamp the health so it doesn't go over the maximum
        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }

        UpdateUI(); // Make sure your bars/icons update!
    }

    private void UpdateUI()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].enabled = (i < currentHealth);
        }
    }
}
