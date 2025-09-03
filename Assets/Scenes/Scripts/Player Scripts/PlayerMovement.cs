using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float dashForce = 10f;
    public GroundCheck groundCheck; 

  
    
    [SerializeField]private bool isDashing = false;
  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        if (!isDashing) HandleMovement(); // ONLY move normally if not dashing
        HandleJump();
        HandleDash();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    void HandleJump()
    {
       
        if (Input.GetKeyDown(KeyCode.W) && groundCheck.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (animator != null) animator.SetTrigger("Jump");
        }
    }
    


    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            rb.velocity = new Vector2(transform.localScale.x * dashForce, rb.velocity.y);
            Invoke(nameof(ResetDash), 0.3f); // dash lasts 0.3s
        }
    }

    void ResetDash()
    {
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal == Vector2.up)
        {
            bool isGrounded;
            isGrounded = true;
        }
    }
}
