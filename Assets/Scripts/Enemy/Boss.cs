using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBlade : Enemy
{
    [Header("Boss AI")]
    public float attackWindupTime = 0.8f;
    public float dashForce = 18f;
    public float restTime = 1.5f;

    [Header("Visuals")]
    public GameObject warningFlash; 
    public Slider bossHealthBar;    
    public float shakeIntensity = 0.2f;

    private bool isAttacking = false;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure moveSpeed is set if you forgot it in the inspector
        if (moveSpeed <= 0) moveSpeed = 3f;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = maxHealth;
        }
    }

    protected override void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange) 
        {
            StartCoroutine(SwordDashAttack());
        }
        else if (dist <= detectionRange) 
        {
            ChasePlayer();
        }
    }

    protected override void ChasePlayer()
    {
        if (player == null || isAttacking) return;

        // Move strictly on X axis toward player
        float dir = (player.position.x > transform.position.x) ? 1 : -1;
        
        // APPLY VELOCITY DIRECTLY
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);

        // FLIP SPRITE
        if (dir > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    IEnumerator SwordDashAttack()
    {
        isAttacking = true;
        
        // STOP moving for windup
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (warningFlash) warningFlash.SetActive(true);
        yield return new WaitForSeconds(attackWindupTime);
        if (warningFlash) warningFlash.SetActive(false);

        // DASH
        Vector2 dashDir = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
        rb.velocity = new Vector2(dashDir.x * dashForce, rb.velocity.y);

        // Shake the camera
        if(CameraShake.Instance != null) CameraShake.Instance.Shake(shakeIntensity, 0.2f);

        // Simple Damage Check
        if (Vector2.Distance(transform.position, player.position) < attackRange)
        {
            player.GetComponent<BatteryHealthUI>()?.TakeDamage(2);
        }

        yield return new WaitForSeconds(0.5f); // Duration of the lunge
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop after dash
        
        yield return new WaitForSeconds(restTime); // Rest state
        isAttacking = false;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (bossHealthBar != null) bossHealthBar.value = currentHealth;
    }

    protected override void Attack() { }
}