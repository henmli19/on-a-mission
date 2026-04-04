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
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            isDead = true;
            audioSource.PlayOneShot(deathSound);

            Player_Scripts.PlayerMovement pm = GetComponent<Player_Scripts.PlayerMovement>();
            if (pm != null) pm.DisableControls();

            QuitMenu.instance.ShowDeathMenu();
            return;
        }

        audioSource.PlayOneShot(damageSound);
    }

    public void RespawnWithHealth(int health)
    {
        isDead = false;
        currentHealth = Mathf.Clamp(health, 1, maxHealth);
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < bars.Length; i++)
            bars[i].enabled = (i < currentHealth);
    }
}