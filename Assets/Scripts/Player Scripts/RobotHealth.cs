using UnityEngine;

public class RobotHealth : MonoBehaviour
{
   
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private Animator animator;
    private bool isDead = false;
    public int MaxHealth => maxHealth;


    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            if (animator != null)
                animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        // Disable your movement script so the robot doesn't keep moving
        GetComponent<PlayerMovement>().enabled = false;

        // Optional: destroy object after animation
        // Destroy(gameObject, 1.5f);
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
